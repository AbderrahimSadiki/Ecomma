-- ============================================
-- BASE DE DONNÉES E-COMMERCE COMPLÈTE (CORRIGÉE)
-- ============================================

-- Suppression des tables si elles existent (dans le bon ordre pour les clés étrangères)
IF OBJECT_ID('OrderTracking', 'U') IS NOT NULL DROP TABLE OrderTracking;
IF OBJECT_ID('OrderItems', 'U') IS NOT NULL DROP TABLE OrderItems;
IF OBJECT_ID('Orders', 'U') IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('Addresses', 'U') IS NOT NULL DROP TABLE Addresses;
IF OBJECT_ID('CartItems', 'U') IS NOT NULL DROP TABLE CartItems;
IF OBJECT_ID('Wishlist', 'U') IS NOT NULL DROP TABLE Wishlist;
IF OBJECT_ID('ProductReviews', 'U') IS NOT NULL DROP TABLE ProductReviews;
IF OBJECT_ID('ProductImages', 'U') IS NOT NULL DROP TABLE ProductImages;
IF OBJECT_ID('ProductVariants', 'U') IS NOT NULL DROP TABLE ProductVariants;
IF OBJECT_ID('Products', 'U') IS NOT NULL DROP TABLE Products;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('DeliveryZones', 'U') IS NOT NULL DROP TABLE DeliveryZones;
IF OBJECT_ID('DeliveryMethods', 'U') IS NOT NULL DROP TABLE DeliveryMethods;
IF OBJECT_ID('StockAlerts', 'U') IS NOT NULL DROP TABLE StockAlerts;
IF OBJECT_ID('StockMovements', 'U') IS NOT NULL DROP TABLE StockMovements;
GO

-- ============================================
-- 1. USERS (avec colonne Role incluse dès la création)
-- ============================================
CREATE TABLE [Users] (
  Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  Email NVARCHAR(256) NOT NULL UNIQUE,
  Phone NVARCHAR(20),
  Name NVARCHAR(150) NOT NULL,
  LastName NVARCHAR(150) NOT NULL,
  PasswordHash NVARCHAR(500),
  IsActive BIT DEFAULT 1,
  CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
  PasswordResetToken NVARCHAR(100) NULL,
  PasswordResetExpiry DATETIME2 NULL,
  [Role] NVARCHAR(50) NOT NULL DEFAULT 'Client'
    CONSTRAINT CK_Users_Role CHECK ([Role] IN ('Admin', 'Client', 'Manager'))
);
GO

-- Index sur le rôle
CREATE INDEX IX_Users_Role ON [Users]([Role]);
GO

-- ============================================
-- 2. CATEGORIES
-- ============================================
CREATE TABLE [Categories] (
  Id INT IDENTITY PRIMARY KEY,
  Name NVARCHAR(150) NOT NULL,
  Slug NVARCHAR(150) NOT NULL UNIQUE,
  ParentId INT NULL,
  FOREIGN KEY (ParentId) REFERENCES Categories(Id)
);
GO

-- ============================================
-- 3. PRODUCTS
-- ============================================
CREATE TABLE [Products] (
  Id INT IDENTITY PRIMARY KEY,
  Name NVARCHAR(250),
  Slug NVARCHAR(250) UNIQUE,
  Description NVARCHAR(MAX),
  Brand NVARCHAR(100),
  CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
  BasePrice DECIMAL(18,2),
  IsFeatured BIT DEFAULT 0,
  CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME()
);
GO

-- ============================================
-- 4. PRODUCT VARIANTS
-- ============================================
CREATE TABLE [ProductVariants] (
  Id INT IDENTITY PRIMARY KEY,
  ProductId INT FOREIGN KEY REFERENCES Products(Id),
  SKU NVARCHAR(100) UNIQUE,
  Attributes NVARCHAR(1000),
  Price DECIMAL(18,2),
  StockQuantity INT DEFAULT 0
);
GO

-- ============================================
-- 5. PRODUCT IMAGES
-- ============================================
CREATE TABLE [ProductImages] (
  Id INT IDENTITY PRIMARY KEY,
  ProductId INT FOREIGN KEY REFERENCES Products(Id),
  ImageUrl NVARCHAR(500) NOT NULL,
  AltText NVARCHAR(200),
  DisplayOrder INT DEFAULT 0,
  IsMainImage BIT DEFAULT 0
);
GO

-- ============================================
-- 6. PRODUCT REVIEWS
-- ============================================
CREATE TABLE [ProductReviews] (
  Id INT IDENTITY PRIMARY KEY,
  ProductId INT FOREIGN KEY REFERENCES Products(Id),
  UserId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(Id),
  Rating INT CHECK (Rating BETWEEN 1 AND 5),
  Comment NVARCHAR(MAX),
  IsVerifiedPurchase BIT DEFAULT 0,
  CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
  UpdatedAt DATETIME2 NULL
);
GO

-- ============================================
-- 7. CART ITEMS
-- ============================================
CREATE TABLE [CartItems] (
    Id INT IDENTITY PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
    ProductVariantId INT NOT NULL FOREIGN KEY REFERENCES ProductVariants(Id),
    Quantity INT DEFAULT 1 CHECK (Quantity > 0),
    UnitPrice DECIMAL(18,2) NOT NULL,
    AddedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UX_CartItems_User_Product UNIQUE(UserId, ProductVariantId)
);
GO

CREATE INDEX IX_CartItems_UserId ON CartItems(UserId);
GO

-- ============================================
-- 8. ADDRESSES
-- ============================================
CREATE TABLE [Addresses] (
  Id INT IDENTITY PRIMARY KEY,
  UserId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(Id),
  Label NVARCHAR(100),
  Street NVARCHAR(300),
  City NVARCHAR(100),
  PostalCode NVARCHAR(20),
  Phone NVARCHAR(20),
  IsDefault BIT
);
GO

CREATE UNIQUE INDEX UX_User_DefaultAddress
ON Addresses(UserId)
WHERE IsDefault = 1;
GO

-- ============================================
-- 9. DELIVERY METHODS
-- ============================================
CREATE TABLE DeliveryMethods (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(10,2) NOT NULL DEFAULT 0,
    EstimatedDays INT NOT NULL DEFAULT 3,
    IsActive BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================
-- 10. DELIVERY ZONES
-- ============================================
CREATE TABLE DeliveryZones (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DeliveryMethodId INT NOT NULL,
    City NVARCHAR(100) NOT NULL,
    Region NVARCHAR(100),
    AdditionalPrice DECIMAL(10,2) NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (DeliveryMethodId) REFERENCES DeliveryMethods(Id) ON DELETE CASCADE
);
GO

-- ============================================
-- 11. ORDERS (avec colonnes livraison intégrées dès la création)
-- ============================================
CREATE TABLE [Orders] (
  Id BIGINT IDENTITY PRIMARY KEY,
  UserId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(Id),
  Total DECIMAL(18,2),
  Tax DECIMAL(18,2),
  Shipping DECIMAL(18,2),
  Status NVARCHAR(50) CHECK (Status IN ('Pending','Processing','Shipped','Delivered','Cancelled')),
  PaymentStatus NVARCHAR(50) CHECK (PaymentStatus IN ('Pending','Paid','Failed','Refunded')),
  PaymentMethod NVARCHAR(50) CHECK (PaymentMethod IN ('CashOnDelivery','CreditCard','BankTransfer')),
  CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
  DeliveryMethodId INT NULL CONSTRAINT FK_Orders_DeliveryMethods FOREIGN KEY REFERENCES DeliveryMethods(Id),
  DeliveryAddress NVARCHAR(500) NULL,
  DeliveryCity NVARCHAR(100) NULL,
  DeliveryPhone NVARCHAR(20) NULL,
  DeliveryFullName NVARCHAR(200) NULL
);
GO

CREATE INDEX IX_Orders_DeliveryMethodId ON Orders(DeliveryMethodId);
GO

-- ============================================
-- 12. ORDER ITEMS
-- ============================================
CREATE TABLE [OrderItems] (
  Id BIGINT IDENTITY PRIMARY KEY,
  OrderId BIGINT FOREIGN KEY REFERENCES Orders(Id),
  ProductVariantId INT FOREIGN KEY REFERENCES ProductVariants(Id),
  Quantity INT,
  UnitPrice DECIMAL(18,2)
);
GO

-- ============================================
-- 13. ORDER TRACKING
-- ============================================
CREATE TABLE [OrderTracking] (
  Id INT PRIMARY KEY IDENTITY(1,1),
  order_id BIGINT NOT NULL,
  Status NVARCHAR(50) NOT NULL CHECK (Status IN ('Pending','Processing','Shipped','Delivered','Cancelled')),
  status_description NVARCHAR(255),
  location NVARCHAR(255),
  created_at DATETIME DEFAULT GETDATE(),
  FOREIGN KEY (order_id) REFERENCES Orders(Id) ON DELETE CASCADE
);
GO

-- ============================================
-- 14. WISHLIST
-- ============================================
CREATE TABLE [Wishlist] (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  UserId UNIQUEIDENTIFIER NOT NULL,
  ProductId INT NOT NULL,
  AddedAt DATETIME2 DEFAULT SYSUTCDATETIME(),
  CONSTRAINT FK_Wishlist_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
  CONSTRAINT FK_Wishlist_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
  CONSTRAINT UX_Wishlist_User_Product UNIQUE(UserId, ProductId)
);
GO

-- ============================================
-- 15. STOCK ALERTS
-- ============================================
CREATE TABLE StockAlerts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductVariantId INT NOT NULL,
    ThresholdQuantity INT NOT NULL DEFAULT 10,
    IsActive BIT NOT NULL DEFAULT 1,
    LastAlertDate DATETIME,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id) ON DELETE CASCADE
);
GO

-- ============================================
-- 16. STOCK MOVEMENTS
-- ============================================
CREATE TABLE StockMovements (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    ProductVariantId INT NOT NULL,
    MovementType NVARCHAR(50) NOT NULL, -- 'IN', 'OUT', 'ADJUSTMENT', 'ORDER', 'RETURN'
    Quantity INT NOT NULL,
    PreviousStock INT NOT NULL,
    NewStock INT NOT NULL,
    Reference NVARCHAR(100),
    Notes NVARCHAR(500),
    CreatedBy UNIQUEIDENTIFIER,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id),
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
);
GO

-- Indexes supplémentaires
CREATE INDEX IX_StockMovements_ProductVariantId ON StockMovements(ProductVariantId);
CREATE INDEX IX_StockMovements_CreatedAt ON StockMovements(CreatedAt DESC);
CREATE INDEX IX_StockAlerts_ProductVariantId ON StockAlerts(ProductVariantId);
CREATE INDEX IX_DeliveryZones_City ON DeliveryZones(City);
GO

PRINT 'Toutes les tables créées avec succès !';
GO

-- ============================================
-- INSERTION DES DONNÉES
-- ============================================

-- USERS
INSERT INTO [Users] (Id, Email, Phone, Name, LastName, PasswordHash, IsActive, [Role]) VALUES
(NEWID(), 'sara.bennani@email.com',       '+212661234567', 'Sara',    'Bennani',    'hash123', 1, 'Admin'),
(NEWID(), 'amina.el.idrissi@email.com',   '+212662345678', 'Amina',   'El Idrissi', 'hash456', 1, 'Client'),
(NEWID(), 'leila.tazi@email.com',         '+212663456789', 'Leila',   'Tazi',       'hash789', 1, 'Client'),
(NEWID(), 'nadia.alami@email.com',        '+212664567890', 'Nadia',   'Alami',      'hash321', 1, 'Client'),
(NEWID(), 'yasmine.chaabi@email.com',     '+212665678901', 'Yasmine', 'Chaabi',     'hash654', 1, 'Client');
GO

-- Admin système
IF NOT EXISTS (SELECT 1 FROM [Users] WHERE Email = 'admin@ecommerce.com')
BEGIN
    INSERT INTO [Users] (Id, Email, Phone, Name, LastName, PasswordHash, IsActive, [Role])
    VALUES (NEWID(), 'admin@ecommerce.com', '+212660000000', 'Admin', 'System', 'admin_hash_password', 1, 'Admin');
    PRINT 'Compte Admin système créé !';
END
GO

-- Admin Oumaima
IF NOT EXISTS (SELECT 1 FROM [Users] WHERE Email = 'oumaima.zerbouhi1@gmail.com')
BEGIN
    INSERT INTO [Users] (Id, Email, Phone, Name, LastName, PasswordHash, IsActive, [Role])
    VALUES (NEWID(), 'oumaima.zerbouhi1@gmail.com', '+212691000000', 'Oumaima', 'Zerbouhi', '12345678', 1, 'Admin');
    PRINT 'Compte Admin Oumaima créé !';
END
GO

-- CATEGORIES
SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] (Id, Name, Slug, ParentId) VALUES
(1,  'Soins du Visage',    'soins-visage',       NULL),
(2,  'Maquillage',         'maquillage',          NULL),
(3,  'Soins du Corps',     'soins-corps',         NULL),
(4,  'Parfums',            'parfums',             NULL),
(5,  'Nettoyants Visage',  'nettoyants-visage',   1),
(6,  'Crèmes Hydratantes', 'cremes-hydratantes',  1),
(7,  'Maquillage Yeux',    'maquillage-yeux',     2),
(8,  'Maquillage Lèvres',  'maquillage-levres',   2),
(9,  'Soins Cheveux',      'soins-cheveux',       NULL),
(10, 'Anti-âge',           'anti-age',            1);
SET IDENTITY_INSERT [Categories] OFF;
GO

-- PRODUCTS
SET IDENTITY_INSERT [Products] ON;
INSERT INTO [Products] (Id, Name, Slug, Description, Brand, CategoryId, BasePrice, IsFeatured) VALUES
(1,  'Sérum Vitamine C Éclat',        'serum-vitamine-c-eclat',        'Sérum concentré à 20% de vitamine C pure pour illuminer le teint, réduire les taches pigmentaires et stimuler la production de collagène. Résultats visibles en 2 semaines.',                         'La Roche-Posay', 1,   349.00, 1),
(3,  'Nettoyant Moussant Doux',        'nettoyant-moussant-doux',        'Gel nettoyant doux qui élimine efficacement maquillage et impuretés sans dessécher. Formule sans savon enrichie en glycérine.',                                                                       'CeraVe',         5,   129.00, 0),
(4,  'Mascara Volume Intense',         'mascara-volume-intense',         'Mascara volumateur effet faux cils avec brosse ergonomique. Formule longue tenue waterproof enrichie en kératine pour fortifier les cils.',                                                             'Maybelline',     7,    99.00, 1),
(6,  'Rouge à Lèvres Mat Longue Tenue','rouge-levres-mat-longue-tenue',  'Rouge à lèvres mat ultra-pigmenté avec tenue 12h. Texture veloutée confortable enrichie en vitamine E. Ne dessèche pas les lèvres.',                                                                  'MAC',            8,   249.00, 0),
(7,  'Gloss Repulpant',                'gloss-repulpant',                'Gloss transparent avec effet repulpant immédiat grâce aux actifs volumateurs. Brillance miroir et parfum vanille subtil.',                                                                             'Dior',           8,   329.00, 0),
(8,  'Eau de Parfum Florale',          'eau-parfum-florale',             'Parfum féminin aux notes de jasmin, rose et musc blanc. Fragrance élégante et raffinée avec tenue longue durée 8-10h.',                                                                                'Chanel',         4,  1299.00, 1),
(9,  'Lait Corps Nourrissant Karité',  'lait-corps-nourrissant-karite',  'Lait corporel ultra-nourrissant au beurre de karité 25%. Pénètre rapidement, hydrate 48h et laisse la peau douce et satinée.',                                                                        'The Body Shop',  3,   179.00, 0),
(10, 'Gommage Corps Sucre Coco',       'gommage-corps-sucre-coco',       'Exfoliant corps naturel aux cristaux de sucre et huile de coco. Élimine les cellules mortes et révèle une peau lisse et lumineuse.',                                                                  'Fresh',          3,   289.00, 0),
(11, 'Shampooing Réparateur Argan',    'shampooing-reparateur-argan',    'Shampooing professionnel à l''huile d''argan du Maroc. Répare les cheveux abîmés, apporte brillance et douceur dès la première utilisation.',                                                          'Moroccanoil',    9,   269.00, 1),
(12, 'Masque Capillaire Nutrition Intense','masque-capillaire-nutrition-intense','Masque sans rinçage ultra-nourrissant pour cheveux secs. Enrichi en kératine et huiles végétales. Application 1-2 fois par semaine.',                                                          'Kérastase',      9,   399.00, 0),
(13, 'Crème Anti-Rides Rétinol',       'creme-anti-rides-retinol',       'Soin anti-âge au rétinol pur qui réduit visiblement rides et ridules. Stimule le renouvellement cellulaire. Résultats après 4 semaines.',                                                             'Olay',           10,  449.00, 1),
(14, 'Contour Yeux Anti-Cernes',       'contour-yeux-anti-cernes',       'Soin ciblé contour des yeux avec applicateur massant. Réduit cernes, poches et ridules. Formule à la caféine et peptides.',                                                                           'Garnier',        10,  159.00, 0);
SET IDENTITY_INSERT [Products] OFF;
GO

-- PRODUCT IMAGES
SET IDENTITY_INSERT [ProductImages] ON;
INSERT INTO [ProductImages] (Id, ProductId, ImageUrl, AltText, DisplayOrder, IsMainImage) VALUES
-- Sérum Vitamine C (1)
(1,  1,  'https://images.unsplash.com/photo-1611930022073-b7a4ba5fcccd?w=600&h=600&fit=crop', 'Sérum Vitamine C vue frontale', 0, 1),
(2,  1,  'https://images.unsplash.com/photo-1620916566398-39f1143ab7be?w=600&h=600&fit=crop', 'Texture du sérum',              1, 0),
(3,  1,  'https://images.unsplash.com/photo-1556228994-56f6a7c0c9b4?w=600&h=600&fit=crop', 'Application du sérum',           2, 0),
-- Nettoyant Moussant (3)
(7,  3,  'https://images.unsplash.com/photo-1571875257727-256c39da42af?w=600&h=600&fit=crop', 'Flacon nettoyant',  0, 1),
(8,  3,  'https://images.unsplash.com/photo-1556228720-195a672e8a03?w=600&h=600&fit=crop', 'Mousse nettoyante', 1, 0),
(9,  3,  'https://images.unsplash.com/photo-1600428650609-b5e56b5c2e64?w=600&h=600&fit=crop', 'Utilisation',       2, 0),
-- Mascara (4)
(10, 4,  'https://images.unsplash.com/photo-1512496015851-a90fb38ba796?w=600&h=600&fit=crop', 'Mascara noir intense', 0, 1),
(11, 4,  'https://images.unsplash.com/photo-1631214524020-7e18db9a8f92?w=600&h=600&fit=crop', 'Brosse mascara',      1, 0),
(12, 4,  'https://images.unsplash.com/photo-1583554801893-46a163d8cc6e?w=600&h=600&fit=crop', 'Résultat volume',     2, 0),
-- Rouge à Lèvres (6)
(16, 6,  'https://images.unsplash.com/photo-1586495777744-4413f21062fa?w=600&h=600&fit=crop', 'Tubes rouge à lèvres', 0, 1),
(17, 6,  'https://images.unsplash.com/photo-1603561591411-07134e71a2a9?w=600&h=600&fit=crop', 'Swatches couleurs',    1, 0),
(18, 6,  'https://images.unsplash.com/photo-1588159343745-445767c4a6ba?w=600&h=600&fit=crop', 'Application lèvres',   2, 0),
-- Gloss (7)
(19, 7,  'https://images.unsplash.com/photo-1596462502278-27bfdc403348?w=600&h=600&fit=crop', 'Gloss repulpant',   0, 1),
(20, 7,  'https://images.unsplash.com/photo-1631730486572-226d1f595b68?w=600&h=600&fit=crop', 'Applicateur',       1, 0),
(21, 7,  'https://images.unsplash.com/photo-1522338242992-e1a54906a8da?w=600&h=600&fit=crop', 'Brillance miroir',  2, 0),
-- Eau de Parfum (8)
(22, 8,  'https://images.unsplash.com/photo-1541643600914-78b084683601?w=600&h=600&fit=crop', 'Flacon eau de parfum', 0, 1),
(23, 8,  'https://images.unsplash.com/photo-1594035910387-fea47794261f?w=600&h=600&fit=crop', 'Vaporisateur',         1, 0),
(24, 8,  'https://images.unsplash.com/photo-1588405748880-12d1d2a59d75?w=600&h=600&fit=crop', 'Mise en scène',        2, 0),
-- Lait Corps (9)
(25, 9,  'https://images.unsplash.com/photo-1608248543803-ba4f8c70ae0b?w=600&h=600&fit=crop', 'Flacon lait corps',   0, 1),
(26, 9,  'https://images.unsplash.com/photo-1556228852-80c3cfc58f7d?w=600&h=600&fit=crop', 'Texture onctueuse',   1, 0),
(27, 9,  'https://images.unsplash.com/photo-1612817288484-6f916006741a?w=600&h=600&fit=crop', 'Application corps',   2, 0),
-- Gommage Corps (10)
(28, 10, 'https://images.unsplash.com/photo-1570554886111-e80fcca6a029?w=600&h=600&fit=crop', 'Pot gommage sucre',   0, 1),
(29, 10, 'https://images.unsplash.com/photo-1598440947619-2c35fc9aa908?w=600&h=600&fit=crop', 'Texture granuleuse',  1, 0),
(30, 10, 'https://images.unsplash.com/photo-1556228994-c7dd2a5a5ca5?w=600&h=600&fit=crop', 'Utilisation gommage', 2, 0),
-- Shampooing (11)
(31, 11, 'https://images.unsplash.com/photo-1535585209827-a15fcdbc4c2d?w=600&h=600&fit=crop', 'Flacon shampooing argan', 0, 1),
(32, 11, 'https://images.unsplash.com/photo-1608248543803-ba4f8c70ae0b?w=600&h=600&fit=crop', 'Texture gel',             1, 0),
(33, 11, 'https://images.unsplash.com/photo-1522338242992-e1a54906a8da?w=600&h=600&fit=crop', 'Cheveux brillants',       2, 0),
-- Masque Capillaire (12)
(34, 12, 'https://images.unsplash.com/photo-1600948836101-f9ffda59d250?w=600&h=600&fit=crop', 'Pot masque capillaire', 0, 1),
(35, 12, 'https://images.unsplash.com/photo-1555529669-e69e7aa0ba9a?w=600&h=600&fit=crop', 'Texture crémeuse',      1, 0),
(36, 12, 'https://images.unsplash.com/photo-1519699047748-de8e457a634e?w=600&h=600&fit=crop', 'Cheveux après soin',    2, 0),
-- Crème Anti-Rides (13)
(37, 13, 'https://images.unsplash.com/photo-1576426863848-c21f53c60b19?w=600&h=600&fit=crop', 'Pot crème rétinol', 0, 1),
(38, 13, 'https://images.unsplash.com/photo-1556228994-1b47b2e4f8b2?w=600&h=600&fit=crop', 'Texture riche',     1, 0),
(39, 13, 'https://images.unsplash.com/photo-1598440946726-e28fabf4dfa1?w=600&h=600&fit=crop', 'Peau lissée',       2, 0),
-- Contour Yeux (14)
(40, 14, 'https://images.unsplash.com/photo-1598440947619-2c35fc9aa908?w=600&h=600&fit=crop', 'Tube contour yeux',     0, 1),
(41, 14, 'https://images.unsplash.com/photo-1556228994-b6c6cc3b9595?w=600&h=600&fit=crop', 'Applicateur massant',   1, 0),
(42, 14, 'https://images.unsplash.com/photo-1515688594390-b649af70d282?w=600&h=600&fit=crop', 'Résultat anti-cernes',  2, 0);
SET IDENTITY_INSERT [ProductImages] OFF;
GO

-- DELIVERY METHODS
INSERT INTO DeliveryMethods (Name, Description, Price, EstimatedDays, IsActive, DisplayOrder) VALUES
('Livraison Standard', 'Livraison à domicile sous 3-5 jours ouvrables', 30.00, 4, 1, 1),
('Livraison Express',  'Livraison à domicile sous 24-48h',               60.00, 1, 1, 2),
('Retrait en magasin', 'Retrait gratuit dans nos points de vente',         0.00, 0, 1, 3);
GO

-- DELIVERY ZONES
INSERT INTO DeliveryZones (DeliveryMethodId, City, Region, AdditionalPrice, IsActive) VALUES
(1, 'Tétouan',     'Tanger-Tétouan-Al Hoceima', 0.00,  1),
(1, 'Tanger',      'Tanger-Tétouan-Al Hoceima', 0.00,  1),
(1, 'Casablanca',  'Casablanca-Settat',          10.00, 1),
(1, 'Rabat',       'Rabat-Salé-Kénitra',         10.00, 1),
(1, 'Marrakech',   'Marrakech-Safi',             15.00, 1),
(2, 'Tétouan',     'Tanger-Tétouan-Al Hoceima', 0.00,  1),
(2, 'Tanger',      'Tanger-Tétouan-Al Hoceima', 0.00,  1),
(2, 'Casablanca',  'Casablanca-Settat',          15.00, 1),
(2, 'Rabat',       'Rabat-Salé-Kénitra',         15.00, 1);
GO

-- PRODUCT VARIANTS (variante par défaut pour chaque produit)
INSERT INTO ProductVariants (ProductId, SKU, Attributes, Price, StockQuantity)
SELECT
    Id,
    'SKU-' + CAST(Id AS NVARCHAR(10)),
    'Standard',
    BasePrice,
    50
FROM Products
WHERE Id NOT IN (SELECT DISTINCT ProductId FROM ProductVariants);
GO

-- STOCK ALERTS
INSERT INTO StockAlerts (ProductVariantId, ThresholdQuantity, IsActive)
SELECT Id, 10, 1
FROM ProductVariants
WHERE Id NOT IN (SELECT ProductVariantId FROM StockAlerts);
GO

-- ============================================
-- VUES
-- ============================================

-- Vue alertes de stock actives
IF OBJECT_ID('vw_ActiveStockAlerts', 'V') IS NOT NULL DROP VIEW vw_ActiveStockAlerts;
GO
CREATE VIEW vw_ActiveStockAlerts AS
SELECT
    sa.Id AS AlertId,
    sa.ProductVariantId,
    pv.SKU,
    pv.Attributes AS VariantName,
    pv.StockQuantity AS CurrentStock,
    sa.ThresholdQuantity,
    p.Id AS ProductId,
    p.Name AS ProductName,
    p.Brand,
    sa.LastAlertDate
FROM StockAlerts sa
INNER JOIN ProductVariants pv ON sa.ProductVariantId = pv.Id
INNER JOIN Products p ON pv.ProductId = p.Id
WHERE sa.IsActive = 1
  AND pv.StockQuantity <= sa.ThresholdQuantity;
GO

-- Vue utilisateurs par rôle
IF OBJECT_ID('vw_UsersByRole', 'V') IS NOT NULL DROP VIEW vw_UsersByRole;
GO
CREATE VIEW vw_UsersByRole AS
SELECT
    Id,
    Email,
    Name,
    LastName,
    Phone,
    [Role],
    IsActive,
    CreatedAt,
    CASE
        WHEN [Role] = 'Admin'   THEN 'Administrateur'
        WHEN [Role] = 'Manager' THEN 'Gestionnaire'
        ELSE 'Client'
    END AS RoleDisplay
FROM [Users];
GO

-- ============================================
-- PROCÉDURES STOCKÉES
-- ============================================

IF OBJECT_ID('sp_CheckUserRole', 'P') IS NOT NULL DROP PROCEDURE sp_CheckUserRole;
GO
CREATE PROCEDURE sp_CheckUserRole
    @Email  NVARCHAR(256),
    @Role   NVARCHAR(50)          OUTPUT,
    @UserId UNIQUEIDENTIFIER      OUTPUT,
    @IsActive BIT                 OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        @Role     = [Role],
        @UserId   = Id,
        @IsActive = IsActive
    FROM [Users]
    WHERE Email = @Email AND IsActive = 1;

    IF @UserId IS NULL
    BEGIN
        SET @Role     = NULL;
        SET @IsActive = 0;
    END
END
GO

-- ============================================
-- VÉRIFICATIONS FINALES
-- ============================================

-- Résumé des images par produit
SELECT
    p.Id AS ProductId,
    p.Name AS ProductName,
    COUNT(pi.Id) AS NombreImages
FROM Products p
LEFT JOIN ProductImages pi ON p.Id = pi.ProductId
GROUP BY p.Id, p.Name
ORDER BY p.Id;

-- Résumé des utilisateurs par rôle
SELECT [Role], COUNT(*) AS NombreUtilisateurs
FROM [Users]
GROUP BY [Role]
ORDER BY [Role];

-- Liste complète des utilisateurs
SELECT
    Id,
    Email,
    Name + ' ' + LastName AS FullName,
    [Role],
    IsActive,
    CreatedAt
FROM [Users]
ORDER BY [Role], CreatedAt;

PRINT '============================================';
PRINT 'Base de données e-commerce créée avec succès !';
PRINT '- 16 tables créées';
PRINT '- 8 utilisateurs (dont 3 Admins)';
PRINT '- 10 catégories';
PRINT '- 12 produits cosmétiques';
PRINT '- 36 images (3 par produit)';
PRINT '- 3 modes de livraison, 9 zones';
PRINT '- Vues et procédures stockées créées';
PRINT '============================================';
