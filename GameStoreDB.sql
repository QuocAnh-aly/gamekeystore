-----------------------------------------------------
-- 0. TẠO DATABASE
-----------------------------------------------------
IF DB_ID('GameStoreDB') IS NOT NULL DROP DATABASE GameStoreDB;

GO
    CREATE DATABASE GameStoreDB;

GO
    USE GameStoreDB;

GO
    -----------------------------------------------------
    -- 1. GENRES
    -----------------------------------------------------
    CREATE TABLE Genres (
        Id INT IDENTITY(1, 1) PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL,
        Slug NVARCHAR(255) NOT NULL
    );

INSERT INTO
    Genres (Name, Slug)
VALUES
    (N'Action', 'action'),
    (N'Adventure', 'adventure'),
    (N'RPG', 'rpg'),
    (N'Strategy', 'strategy'),
    (N'Simulation', 'simulation'),
    (N'Indie', 'indie'),
    (N'Survival', 'survival'),
    (N'Horror', 'horror');

-----------------------------------------------------
-- 2. PLATFORMS
-----------------------------------------------------
CREATE TABLE Platforms (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Slug NVARCHAR(255) NOT NULL
);

INSERT INTO
    Platforms (Name, Slug)
VALUES
    (N'Steam', 'steam'),
    (N'Epic Games', 'epic-games'),
    (N'Origin', 'origin'),
    (N'Uplay', 'uplay'),
    (N'PS5', 'ps5'),
    (N'Xbox', 'xbox');

-----------------------------------------------------
-- 3. PRODUCTS
-----------------------------------------------------
CREATE TABLE Products (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Slug NVARCHAR(255) NOT NULL,
    ShortDescription NVARCHAR(MAX),
    Description NVARCHAR(MAX),
    Price DECIMAL(18, 2),
    SalePrice DECIMAL(18, 2),
    CoverImage NVARCHAR(MAX),
    IsNew BIT DEFAULT 0,
    IsHot BIT DEFAULT 0,
    IsFeatured BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);

INSERT INTO
    Products (
        Name,
        Slug,
        ShortDescription,
        Description,
        Price,
        SalePrice,
        CoverImage,
        IsNew,
        IsHot,
        IsFeatured
    )
VALUES
    (
        N'Elden Ring',
        'elden-ring',
        N'Hành động RPG trong thế giới mở dark fantasy.',
        N 'THE NEW FANTASY ACTION RPG. Rise, Tarnished...',
        1200000,
        950000,
        'https://image.api.playstation.com/vulcan/ap/rnd/202108/0410/0JmO1x9l5k2iK4iK64Xp3k0A.jpg',
        1,
        1,
        1
    ),
    (
        N'Cyberpunk 2077',
        'cyberpunk-2077',
        N 'Phiêu lưu hành động trong thế giới mở Night City.',
        N 'Open-world action-adventure story set in Night City.',
        1100000,
        NULL,
        'https://image.api.playstation.com/vulcan/ap/rnd/202211/0711/kh4MUIu43B45iRmaCqu8S5wQ.jpg',
        0,
        1,
        0
    );

-----------------------------------------------------
-- 4. PRODUCT GENRES
-----------------------------------------------------
CREATE TABLE ProductGenres (
    ProductId INT,
    GenreId INT,
    PRIMARY KEY (ProductId, GenreId),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (GenreId) REFERENCES Genres(Id)
);

INSERT INTO
    ProductGenres
VALUES
    (1, 1),
    (1, 3),
    (2, 1),
    (2, 3);

-----------------------------------------------------
-- 5. PRODUCT PLATFORMS
-----------------------------------------------------
CREATE TABLE ProductPlatforms (
    ProductId INT,
    PlatformId INT,
    PRIMARY KEY (ProductId, PlatformId),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (PlatformId) REFERENCES Platforms(Id)
);

INSERT INTO
    ProductPlatforms
VALUES
    (1, 1),
    (1, 2),
    (2, 1);

-----------------------------------------------------
-- 6. PRODUCT IMAGES
-----------------------------------------------------
CREATE TABLE ProductImages (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    ProductId INT,
    ImageUrl NVARCHAR(MAX),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

INSERT INTO
    ProductImages
VALUES
    (
        1,
        'https://image.api.playstation.com/vulcan/ap/rnd/202108/0410/0JmO1x9l5k2iK4iK64Xp3k0A.jpg'
    ),
    (
        1,
        'https://image.api.playstation.com/vulcan/ap/rnd/202108/0410/T0sCNyT1wG2sC7I0d6pLbeom.jpg'
    ),
    (
        2,
        'https://image.api.playstation.com/vulcan/ap/rnd/202211/0711/kh4MUIu43B45iRmaCqu8S5wQ.jpg'
    );

-----------------------------------------------------
-- 7. USERS
-----------------------------------------------------
CREATE TABLE Users (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE,
    Password NVARCHAR(255),
    Role NVARCHAR(50) DEFAULT 'user',
    IsVerified BIT DEFAULT 0,
    GoogleId NVARCHAR(255),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);

-----------------------------------------------------
-- 8. WISHLIST
-----------------------------------------------------
CREATE TABLE UserWishlists (
    UserId INT,
    ProductId INT,
    PRIMARY KEY (UserId, ProductId),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-----------------------------------------------------
-- 9. INVENTORY (GAME KEYS)
-----------------------------------------------------
CREATE TABLE Inventories (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    ProductId INT NOT NULL,
    PlatformId INT NOT NULL,
    GameKey NVARCHAR(255) UNIQUE,
    IsSold BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (PlatformId) REFERENCES Platforms(Id)
);

-----------------------------------------------------
-- 10. ORDERS
-----------------------------------------------------
CREATE TABLE Orders (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    UserId INT,
    TotalPrice DECIMAL(18, 2),
    PaymentMethod NVARCHAR(100),
    Status NVARCHAR(50) DEFAULT 'pending',
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-----------------------------------------------------
-- 11. ORDER ITEMS
-----------------------------------------------------
CREATE TABLE OrderItems (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    OrderId INT,
    ProductId INT,
    PlatformId INT,
    PriceAtPurchase DECIMAL(18, 2),
    PurchasedKeyId INT,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (PlatformId) REFERENCES Platforms(Id),
    FOREIGN KEY (PurchasedKeyId) REFERENCES Inventories(Id)
);
