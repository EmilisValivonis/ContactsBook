USE [ContactsDB]
GO
/****** Object:  Table [dbo].[Contacts]    Script Date: 9/4/2023 9:17:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contacts](
	[ContactID] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](100) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[DateOfBirth] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[ContactID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[DeleteContact]    Script Date: 9/4/2023 9:17:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Stored procedure for deleting a contact
CREATE PROCEDURE [dbo].[DeleteContact]
    @ContactID INT
AS
BEGIN
    DELETE FROM dbo.Contacts
    WHERE ContactID = @ContactID
END
GO
/****** Object:  StoredProcedure [dbo].[GetContactByID]    Script Date: 9/4/2023 9:17:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Stored procedure for retrieving a contact by ID
CREATE PROCEDURE [dbo].[GetContactByID]
    @ContactID INT
AS
BEGIN
    SELECT ContactID, FullName, PhoneNumber, DateOfBirth
    FROM dbo.Contacts
    WHERE ContactID = @ContactID
END
COMMIT; 
GO
/****** Object:  StoredProcedure [dbo].[GetContacts]    Script Date: 9/4/2023 9:17:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Stored procedure for retrieving all contacts
CREATE PROCEDURE [dbo].[GetContacts]
AS
BEGIN
    SELECT ContactID, FullName, PhoneNumber, DateOfBirth
    FROM dbo.Contacts;
END
GO
/****** Object:  StoredProcedure [dbo].[InsertContact]    Script Date: 9/4/2023 9:17:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- Stored procedure for inserting a contact
CREATE PROCEDURE [dbo].[InsertContact]
    @FullName NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @DateOfBirth DATE
AS
BEGIN
    INSERT INTO dbo.Contacts (FullName, PhoneNumber, DateOfBirth)
    VALUES (@FullName, @PhoneNumber, @DateOfBirth)
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateContact]    Script Date: 9/4/2023 9:17:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Stored procedure for updating a contact
CREATE PROCEDURE [dbo].[UpdateContact]
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
END
GO
