CREATE TABLE [dbo].[Ingredients]
(
	[IngredientId] INT IDENTITY(1, 1) NOT NULL,
	[UserId] INT NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	/*This is where it gets tricky and will need copious amounts of redos. I'm unsure what works best for data coherency. */
	[Unit] NVARCHAR(50) NOT NULL,
	[Category] NVARCHAR(50) NOT NULL,
	--[ExpirationDate] DATETIME NOT NULL,
	[Quantity] FLOAT NOT NULL,
	[DateAdded] DATETIME NOT NULL DEFAULT GETDATE(),
	PRIMARY KEY CLUSTERED ([IngredientId] ASC),
	FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])
)
