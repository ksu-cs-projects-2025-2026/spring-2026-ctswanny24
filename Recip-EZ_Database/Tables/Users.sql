CREATE TABLE [dbo].[Users]
(
	/*	Simplest of the tables. Simply holds the user information. For security reasons, the password is a hash
		The user will have an inventory of many ingredient items and so the ingredients will have a foreign key to the user.
	*/
	[UserId] INT IDENTITY(1,1),
	[Username] NVARCHAR(255) NOT NULL,
	[Password] NVARCHAR(255) NOT NULL,
	[FirstName] NVARCHAR(255) NOT NULL,
	[LastName] NVARCHAR(255) NOT NULL,
	[CreatedOn] DATETIME NOT NULL DEFAULT GETDATE(),
	UNIQUE([Username], [Password]),
	PRIMARY KEY CLUSTERED ([UserId] ASC)
)
