IF OBJECT_ID('dbo.Video_Games', 'U') IS NOT NULL
    DROP TABLE dbo.Video_Games;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
    DROP TABLE dbo.Users;
GO

CREATE TABLE dbo.Users
(
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    email NVARCHAR(255) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    street_address NVARCHAR(255) NOT NULL,
);
GO

CREATE TABLE dbo.Video_Games
(
    game_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    name NVARCHAR(255) NOT NULL,
    publisher NVARCHAR(255) NOT NULL,
    year_published INT NOT NULL,
    system NVARCHAR(100) NOT NULL,
    previous_owners_count INT NULL,
    [condition] NVARCHAR(10) NOT NULL CHECK ([condition] IN ('mint','good','fair','poor')),
    CONSTRAINT FK_VideoGames_Users FOREIGN KEY (user_id)
        REFERENCES dbo.Users(user_id) ON DELETE CASCADE
);
GO