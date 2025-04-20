-- Create Users table
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    PasswordSalt NVARCHAR(MAX) NOT NULL,
    MonthlySalary DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    LastLoginAt DATETIME NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    RefreshToken NVARCHAR(MAX) NULL,
    RefreshTokenExpiryTime DATETIME NULL,
    ProfilePicture NVARCHAR(MAX) NULL,
    Language NVARCHAR(10) NOT NULL DEFAULT 'en',
    Theme NVARCHAR(20) NOT NULL DEFAULT 'light'
);

-- Create UserPreferences table
CREATE TABLE UserPreferences (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ItemsPerPage INT NOT NULL DEFAULT 10,
    DefaultCurrency NVARCHAR(3) NOT NULL DEFAULT 'USD',
    NotificationEnabled BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Create Categories table
CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    UserId INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Create Expenses table
CREATE TABLE Expenses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Date DATETIME NOT NULL,
    CategoryId INT NOT NULL,
    UserId INT NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Create indexes
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Categories_UserId ON Categories(UserId);
CREATE INDEX IX_Expenses_UserId ON Expenses(UserId);
CREATE INDEX IX_Expenses_CategoryId ON Expenses(CategoryId);
CREATE INDEX IX_Expenses_Date ON Expenses(Date);
CREATE INDEX IX_UserPreferences_UserId ON UserPreferences(UserId); 