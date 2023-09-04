-- Check if the 'InsertContact' stored procedure exists; if not, create it
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertContact]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
   CREATE PROCEDURE InsertContact
        @FullName NVARCHAR(100),
        @PhoneNumber NVARCHAR(20),
        @DateOfBirth DATE
    AS
    BEGIN
        INSERT INTO Contacts (FullName, PhoneNumber, DateOfBirth)
        VALUES (@FullName, @PhoneNumber, @DateOfBirth);
    END
    ')
END
GO
-- Check if the 'UpdateContact' stored procedure exists; if not, create it
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateContact]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
    CREATE PROCEDURE UpdateContact
        @ContactID INT,
        @FullName NVARCHAR(100),
        @PhoneNumber NVARCHAR(20),
        @DateOfBirth DATE
    AS
    BEGIN
        UPDATE Contacts
        SET FullName = @FullName,
            PhoneNumber = @PhoneNumber,
            DateOfBirth = @DateOfBirth
        WHERE ContactID = @ContactID
    END')
END
GO

-- Check if the 'DeleteContact' stored procedure exists; if not, create it
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteContact]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
    CREATE PROCEDURE DeleteContact
        @ContactID INT
    AS
    BEGIN
        DELETE FROM dbo.Contacts
        WHERE ContactID = @ContactID
    END')
END
GO

-- Check if the 'GetContacts' stored procedure exists; if not, create it
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetContacts]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
    CREATE PROCEDURE GetContacts
    AS
    BEGIN
        SELECT ContactID, FullName, PhoneNumber, DateOfBirth
        FROM dbo.Contacts;
    END')
END
GO

-- Check if the 'GetContactByID' stored procedure exists; if not, create it
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetContactByID]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
    CREATE PROCEDURE GetContactByID
        @ContactID INT
    AS
    BEGIN
        SELECT ContactID, FullName, PhoneNumber, DateOfBirth
        FROM dbo.Contacts
        WHERE ContactID = @ContactID
    END')
END
GO
