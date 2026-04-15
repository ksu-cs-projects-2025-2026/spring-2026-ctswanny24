CREATE TABLE [dbo].[UserInventory]
(
	[UserInventoryId] INT IDENTITY(1, 1) NOT NULL,
	[UserId] INT NOT NULL,
	[IngredientId] INT NOT NULL,
	[Unit] NVARCHAR(50) NOT NULL,
	[Category] NVARCHAR(50) NOT NULL,
	[Quantity] FLOAT NOT NULL,
	[DateAdded] DATETIME NOT NULL DEFAULT GETDATE(),
	[ExpirationDate] DATETIME NULL,
	PRIMARY KEY CLUSTERED ([UserInventoryId] ASC),
	FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]),
	FOREIGN KEY ([IngredientId]) REFERENCES [dbo].[Ingredient]([IngredientId])
)
