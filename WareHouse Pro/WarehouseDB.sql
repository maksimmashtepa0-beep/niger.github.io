USE master;
GO

IF DB_ID('WarehouseDB') IS NOT NULL
BEGIN
    ALTER DATABASE WarehouseDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE WarehouseDB;
END
GO

CREATE DATABASE WarehouseDB;
GO

USE WarehouseDB;
GO

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Article NVARCHAR(50) NULL,
    Category NVARCHAR(100) NULL,
    Quantity INT NOT NULL DEFAULT 0,
    MinimumStock INT NOT NULL DEFAULT 0
);

CREATE TABLE Operations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    Type NVARCHAR(30) NOT NULL CHECK (Type IN (N'Приход', N'Расход', N'Корректировка', N'Возврат от клиента', N'Возврат поставщику')),
    Quantity INT NOT NULL,
    Date DATETIME NOT NULL DEFAULT GETDATE(),
    OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
    CONSTRAINT FK_Operations_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
GO

INSERT INTO Products (Name, Article, Category, Quantity, MinimumStock) VALUES
(N'Шкаф офисный', N'SH-01', N'Мебель', 20, 5),
(N'Стол рабочий', N'TBL-12', N'Мебель', 15, 4),
(N'Стул', N'CHR-07', N'Мебель', 35, 10),
(N'Компьютер', N'PC-220', N'Техника', 8, 3),
(N'Принтер', N'PRN-90', N'Техника', 6, 2);

INSERT INTO Operations (ProductId, Type, Quantity, Date, OrderNumber) VALUES
(1, N'Приход', 10, '2026-09-01 09:00:00', N'ORD-20260901-0001'),
(1, N'Расход', 3, '2026-09-02 11:00:00', N'ORD-20260902-0002'),
(2, N'Приход', 8, '2026-09-03 10:00:00', N'ORD-20260903-0003'),
(2, N'Расход', 2, '2026-09-04 15:00:00', N'ORD-20260904-0004'),
(3, N'Приход', 20, '2026-09-05 12:00:00', N'ORD-20260905-0005'),
(4, N'Приход', 5, '2026-09-06 09:30:00', N'ORD-20260906-0006'),
(4, N'Расход', 2, '2026-09-07 10:00:00', N'ORD-20260907-0007'),
(5, N'Приход', 4, '2026-09-08 13:00:00', N'ORD-20260908-0008');
GO

SELECT * FROM Products;
SELECT * FROM Operations;
GO
