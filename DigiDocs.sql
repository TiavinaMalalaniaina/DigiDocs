
-- ============================
-- TABLE : Users
-- ============================
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role NVARCHAR(50) NOT NULL
);
GO

-- ============================
-- TABLE : Documents
-- ============================
CREATE TABLE Documents (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UploadDate DATETIME NOT NULL DEFAULT GETDATE(),
    DownloadCount INT NOT NULL DEFAULT 0,

    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,

    FileData VARBINARY(MAX) NOT NULL,
    FileName NVARCHAR(300) NOT NULL,
    ContentType NVARCHAR(100) NOT NULL,

    AccessLevel NVARCHAR(50) NOT NULL,
    Category NVARCHAR(50) NOT NULL
);
GO

-- ============================
-- TABLE : DocumentDownloads
-- Relation entre Users & Documents
-- ============================
CREATE TABLE DocumentDownloads (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    
    DocumentId INT NOT NULL,
    UserId INT NOT NULL,
    DownloadDate DATETIME NOT NULL DEFAULT GETDATE(),

    -- Foreign keys
    CONSTRAINT FK_DocumentDownloads_Document
        FOREIGN KEY (DocumentId) REFERENCES Documents(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_DocumentDownloads_User
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE CASCADE
);
GO




UPDATE Documents
SET DownloadCount = (
    SELECT COUNT(*)
    FROM DocumentDownloads
    WHERE DocumentId = Documents.Id
);


DBCC CHECKIDENT ('NomDeLaTable', RESEED, 0);