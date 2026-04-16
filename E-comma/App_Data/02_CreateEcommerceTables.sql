-- =============================================
-- Script de Création des Tables E-commerce
-- =============================================

-- Table: Categories
CREATE TABLE [dbo].[Categories] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX),
    [IsActive] BIT DEFAULT 1,
    [CreatedAt] DATETIME DEFAULT GETDATE()
);

-- Table: Products
CREATE TABLE [dbo].[Products] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [CategoryId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX),
    [Price] DECIMAL(18, 2) NOT NULL,
    [ImageUrl] NVARCHAR(500),
    [Stock] INT DEFAULT 0,
    [IsActive] BIT DEFAULT 1,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories]([Id])
);

-- Table: ShoppingCarts
CREATE TABLE [dbo].[ShoppingCarts] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
);

-- Table: ShoppingCartItems
CREATE TABLE [dbo].[ShoppingCartItems] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [ShoppingCartId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Quantity] INT NOT NULL DEFAULT 1,
    [Price] DECIMAL(18, 2) NOT NULL,
    [AddedAt] DATETIME DEFAULT GETDATE(),
    FOREIGN KEY ([ShoppingCartId]) REFERENCES [dbo].[ShoppingCarts]([Id]),
    FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id])
);

-- Table: Orders
CREATE TABLE [dbo].[Orders] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [OrderNumber] NVARCHAR(50) NOT NULL UNIQUE,
    [OrderDate] DATETIME DEFAULT GETDATE(),
    [Status] NVARCHAR(50) DEFAULT 'Pending', -- Pending, Completed, Cancelled
    [TotalAmount] DECIMAL(18, 2) NOT NULL,
    [ShippingAddress] NVARCHAR(MAX) NOT NULL,
    [CustomerName] NVARCHAR(200) NOT NULL,
    [CustomerEmail] NVARCHAR(256) NOT NULL,
    [CustomerPhone] NVARCHAR(50),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
);

-- Table: OrderItems
CREATE TABLE [dbo].[OrderItems] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [OrderId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [ProductName] NVARCHAR(200) NOT NULL,
    [Quantity] INT NOT NULL,
    [UnitPrice] DECIMAL(18, 2) NOT NULL,
    [TotalPrice] DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders]([Id]),
    FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id])
);

-- =============================================
-- Index pour améliorer les performances
-- =============================================

CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_ShoppingCarts_UserId ON ShoppingCarts(UserId);
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
CREATE INDEX IX_Orders_OrderNumber ON Orders(OrderNumber);
