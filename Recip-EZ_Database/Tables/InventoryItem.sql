CREATE TABLE [dbo].[Ingredient]
(
	[IngredientId] INT IDENTITY(1, 1) NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Category] NVARCHAR(50) NOT NULL,
	PRIMARY KEY CLUSTERED ([IngredientId] ASC),
	)
