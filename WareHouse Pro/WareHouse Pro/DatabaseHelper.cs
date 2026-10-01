using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace WareHouse_Pro
{
    public static class DatabaseHelper
    {
        private static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            }
        }

        public static bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    EnsureDatabaseSchema(connection);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void EnsureDatabaseSchema(SqlConnection connection)
        {
            ExecuteSchemaCommand(connection, "IF OBJECT_ID(N'dbo.Products', N'U') IS NULL RAISERROR('Таблица Products не найдена. Выполните WarehouseDB.sql.', 16, 1);");
            ExecuteSchemaCommand(connection, "IF COL_LENGTH(N'dbo.Products', N'Article') IS NULL ALTER TABLE dbo.Products ADD Article NVARCHAR(50) NULL;");
            ExecuteSchemaCommand(connection, "IF COL_LENGTH(N'dbo.Products', N'Category') IS NULL ALTER TABLE dbo.Products ADD Category NVARCHAR(100) NULL;");
            ExecuteSchemaCommand(connection, "IF COL_LENGTH(N'dbo.Products', N'MinimumStock') IS NULL ALTER TABLE dbo.Products ADD MinimumStock INT NOT NULL CONSTRAINT DF_Products_MinimumStock DEFAULT (0) WITH VALUES;");
        }

        private static void ExecuteSchemaCommand(SqlConnection connection, string sql)
        {
            using (var command = new SqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void RepairLegacyTextEncoding(SqlConnection connection)
        {
            var products = new DataTable();
            using (var command = new SqlCommand("SELECT Id, Name, Article, Category FROM Products", connection))
            using (var reader = command.ExecuteReader())
            {
                products.Load(reader);
            }

            foreach (DataRow row in products.Rows)
            {
                string name = RepairText(row["Name"]?.ToString());
                string article = RepairText(row["Article"] == DBNull.Value ? null : row["Article"].ToString());
                string category = RepairText(row["Category"] == DBNull.Value ? null : row["Category"].ToString());

                if (name != row["Name"].ToString() || article != (row["Article"] == DBNull.Value ? null : row["Article"].ToString()) || category != (row["Category"] == DBNull.Value ? null : row["Category"].ToString()))
                {
                    using (var command = new SqlCommand(
                        "UPDATE Products SET Name = @name, Article = @article, Category = @category WHERE Id = @id", connection))
                    {
                        command.Parameters.Add("@id", SqlDbType.Int).Value = row["Id"];
                        command.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                        command.Parameters.Add("@article", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(article) ? (object)DBNull.Value : article;
                        command.Parameters.Add("@category", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(category) ? (object)DBNull.Value : category;
                        command.ExecuteNonQuery();
                    }
                }
            }

            var operations = new DataTable();
            using (var command = new SqlCommand("SELECT Id, Type FROM Operations", connection))
            using (var reader = command.ExecuteReader())
            {
                operations.Load(reader);
            }

            foreach (DataRow row in operations.Rows)
            {
                string type = RepairText(row["Type"]?.ToString());
                if (type != row["Type"].ToString())
                {
                    using (var command = new SqlCommand("UPDATE Operations SET Type = @type WHERE Id = @id", connection))
                    {
                        command.Parameters.Add("@id", SqlDbType.Int).Value = row["Id"];
                        command.Parameters.Add("@type", SqlDbType.NVarChar).Value = type;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static string RepairText(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.Contains("Р") || !value.Contains("С"))
            {
                return value;
            }

            try
            {
                string repaired = Encoding.UTF8.GetString(Encoding.GetEncoding(1251).GetBytes(value));
                return repaired.Contains("�") ? value : repaired;
            }
            catch
            {
                return value;
            }
        }

        public static DataTable GetProductsTable(string search = "")
        {
            var table = new DataTable();
            string query = @"
                SELECT Id,
                       Name,
                       Article,
                       Category,
                       Quantity,
                       MinimumStock
                FROM Products
                WHERE @search = ''
                   OR Name LIKE @pattern
                   OR Article LIKE @pattern
                   OR Category LIKE @pattern
                ORDER BY Name";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@search", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(search) ? string.Empty : search;
                command.Parameters.Add("@pattern", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(search) ? string.Empty : "%" + search + "%";

                connection.Open();
                EnsureDatabaseSchema(connection);
                using (var reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table;
        }

        public static DataTable GetProductsForCombo()
        {
            var table = new DataTable();
            string query = "SELECT Id, Name FROM Products ORDER BY Name";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                EnsureDatabaseSchema(connection);
                using (var reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table;
        }

        public static int AddProduct(string name, int quantity, string article = "", string category = "", int minimumStock = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Название товара не может быть пустым.");
            }

            if (quantity < 0)
            {
                throw new Exception("Количество товара не может быть отрицательным.");
            }

            if (minimumStock < 0)
            {
                throw new Exception("Минимальный остаток не может быть отрицательным.");
            }

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(
                "INSERT INTO Products (Name, Article, Category, Quantity, MinimumStock) VALUES (@name, @article, @category, @quantity, @minimumStock)", connection))
            {
                command.Parameters.Add("@name", SqlDbType.NVarChar).Value = name.Trim();
                command.Parameters.Add("@article", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(article) ? (object)DBNull.Value : article.Trim();
                command.Parameters.Add("@category", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(category) ? (object)DBNull.Value : category.Trim();
                command.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                command.Parameters.Add("@minimumStock", SqlDbType.Int).Value = minimumStock;

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int UpdateProduct(int productId, string name, int quantity, string article = "", string category = "", int minimumStock = 0)
        {
            if (productId <= 0)
            {
                throw new Exception("Некорректный идентификатор товара.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Название товара не может быть пустым.");
            }

            if (quantity < 0)
            {
                throw new Exception("Количество товара не может быть отрицательным.");
            }

            if (minimumStock < 0)
            {
                throw new Exception("Минимальный остаток не может быть отрицательным.");
            }

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(
                "UPDATE Products SET Name = @name, Article = @article, Category = @category, Quantity = @quantity, MinimumStock = @minimumStock WHERE Id = @id", connection))
            {
                command.Parameters.Add("@id", SqlDbType.Int).Value = productId;
                command.Parameters.Add("@name", SqlDbType.NVarChar).Value = name.Trim();
                command.Parameters.Add("@article", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(article) ? (object)DBNull.Value : article.Trim();
                command.Parameters.Add("@category", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(category) ? (object)DBNull.Value : category.Trim();
                command.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                command.Parameters.Add("@minimumStock", SqlDbType.Int).Value = minimumStock;

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int DeleteProduct(int productId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand("DELETE FROM Products WHERE Id = @id", connection))
            {
                command.Parameters.Add("@id", SqlDbType.Int).Value = productId;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static void AddOperation(int productId, string type, int quantity)
        {
            if (quantity <= 0)
            {
                throw new Exception("Количество должно быть больше нуля.");
            }

            if (string.IsNullOrWhiteSpace(type) ||
                (type != "Приход" && type != "Расход" && type != "Корректировка" &&
                 type != "Возврат от клиента" && type != "Возврат поставщику"))
            {
                throw new Exception("Некорректный тип операции.");
            }

            var orderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmssfff}-{productId}";

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var selectCommand = new SqlCommand(
                            "SELECT Quantity FROM Products WHERE Id = @productId", connection, transaction))
                        {
                            selectCommand.Parameters.Add("@productId", SqlDbType.Int).Value = productId;
                            var currentQuantity = selectCommand.ExecuteScalar();

                            if (currentQuantity == null)
                            {
                                throw new Exception("Товар не найден.");
                            }

                            int stock = Convert.ToInt32(currentQuantity);

                            bool decreasesStock = type == "Расход" || type == "Возврат поставщику";
                            if (decreasesStock && stock < quantity)
                            {
                                throw new Exception("Недостаточно товара на складе.");
                            }
                        }

                        using (var updateCommand = new SqlCommand(
                            "UPDATE Products SET Quantity = Quantity + @delta WHERE Id = @productId", connection, transaction))
                        {
                            updateCommand.Parameters.Add("@productId", SqlDbType.Int).Value = productId;
                            bool decreasesStock = type == "Расход" || type == "Возврат поставщику";
                            updateCommand.Parameters.Add("@delta", SqlDbType.Int).Value = decreasesStock ? -quantity : quantity;
                            updateCommand.ExecuteNonQuery();
                        }

                        using (var insertCommand = new SqlCommand(
                            "INSERT INTO Operations (ProductId, Type, Quantity, Date, OrderNumber) VALUES (@productId, @type, @quantity, @date, @orderNumber)", connection, transaction))
                        {
                            insertCommand.Parameters.Add("@productId", SqlDbType.Int).Value = productId;
                            insertCommand.Parameters.Add("@type", SqlDbType.NVarChar).Value = type;
                            insertCommand.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                            insertCommand.Parameters.Add("@date", SqlDbType.DateTime).Value = DateTime.Now;
                            insertCommand.Parameters.Add("@orderNumber", SqlDbType.NVarChar, 50).Value = orderNumber;
                            insertCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (SqlException ex) when (ex.Number == 2627)
                    {
                        transaction.Rollback();
                        throw new Exception("Такой заказ уже существует. Повторное сохранение запрещено.");
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void AdjustStock(int productId, int delta, string reason = "")
        {
            if (productId <= 0)
            {
                throw new Exception("Некорректный идентификатор товара.");
            }

            if (delta == 0)
            {
                throw new Exception("Корректировка не может быть нулевой.");
            }

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var selectCommand = new SqlCommand(
                            "SELECT Quantity FROM Products WHERE Id = @productId", connection, transaction))
                        {
                            selectCommand.Parameters.Add("@productId", SqlDbType.Int).Value = productId;
                            var currentQuantity = selectCommand.ExecuteScalar();

                            if (currentQuantity == null)
                            {
                                throw new Exception("Товар не найден.");
                            }

                            int stock = Convert.ToInt32(currentQuantity);
                            if (stock + delta < 0)
                            {
                                throw new Exception("После корректировки остаток не может быть отрицательным.");
                            }
                        }

                        using (var updateCommand = new SqlCommand(
                            "UPDATE Products SET Quantity = Quantity + @delta WHERE Id = @productId", connection, transaction))
                        {
                            updateCommand.Parameters.Add("@productId", SqlDbType.Int).Value = productId;
                            updateCommand.Parameters.Add("@delta", SqlDbType.Int).Value = delta;
                            updateCommand.ExecuteNonQuery();
                        }

                        using (var insertCommand = new SqlCommand(
                            "INSERT INTO Operations (ProductId, Type, Quantity, Date, OrderNumber) VALUES (@productId, @type, @quantity, @date, @orderNumber)", connection, transaction))
                        {
                            insertCommand.Parameters.Add("@productId", SqlDbType.Int).Value = productId;
                            insertCommand.Parameters.Add("@type", SqlDbType.NVarChar).Value = "Корректировка";
                            insertCommand.Parameters.Add("@quantity", SqlDbType.Int).Value = Math.Abs(delta);
                            insertCommand.Parameters.Add("@date", SqlDbType.DateTime).Value = DateTime.Now;
                            insertCommand.Parameters.Add("@orderNumber", SqlDbType.NVarChar, 50).Value = $"ADJ-{DateTime.Now:yyyyMMddHHmmssfff}-{productId}";
                            insertCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static DataTable GetTopProducts()
        {
            var table = new DataTable();
            string query = @"
                SELECT TOP 5 p.Name,
                       SUM(CASE WHEN o.Type = 'Приход' THEN o.Quantity
                                WHEN o.Type = 'Расход' THEN -o.Quantity
                                ELSE 0 END) AS NetMovement
                FROM Products p
                LEFT JOIN Operations o ON o.ProductId = p.Id
                GROUP BY p.Name
                ORDER BY NetMovement DESC";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table;
        }

        public static DataTable GetMonthlyMovementData()
        {
            var table = new DataTable();
            string query = @"
                SELECT DATENAME(month, Date) AS MonthName,
                       MONTH(Date) AS MonthNumber,
                       SUM(CASE WHEN Type = 'Приход' THEN Quantity
                                WHEN Type = 'Расход' THEN -Quantity
                                ELSE 0 END) AS NetMovement
                FROM Operations
                GROUP BY MONTH(Date), DATENAME(month, Date)
                ORDER BY MONTH(Date)";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table;
        }

        public static DataTable GetChartTopProducts()
        {
            return GetTopProducts();
        }

        public static DataTable GetLowStockProducts()
        {
            var table = new DataTable();
            string query = @"
                SELECT TOP 10
                    Name,
                    Quantity,
                    MinimumStock,
                    (Quantity - MinimumStock) AS Difference
                FROM Products
                WHERE Quantity <= ISNULL(MinimumStock, 10)
                ORDER BY Quantity ASC";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table;
        }

        public static DataTable GetOperationsTable(string search = "")
        {
            var table = new DataTable();
            string query = @"
                SELECT o.Id,
                       p.Name AS ProductName,
                       o.Type,
                       o.Quantity,
                       o.Date,
                       o.OrderNumber
                FROM Operations o
                INNER JOIN Products p ON p.Id = o.ProductId
                WHERE (@search = '' OR p.Name LIKE @pattern OR o.OrderNumber LIKE @pattern)
                ORDER BY o.Date DESC, o.Id DESC";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@search", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(search) ? string.Empty : search;
                command.Parameters.Add("@pattern", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(search) ? string.Empty : "%" + search + "%";

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table;
        }
    }
}
