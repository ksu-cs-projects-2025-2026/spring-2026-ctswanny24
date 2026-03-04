CREATE TABLE [dbo].[RecipeIngredients]
(
	[RecipeIngredientsId] INT,
	[RecipeId] INT NOT NULL,
	[IngredientName] NVARCHAR(255) NOT NULL,
	[Quantity] FLOAT NOT NULL,
	[Unit] NVARCHAR(50) NOT NULL,
	[PreparationInstructions] NVARCHAR(255) NULL,
	PRIMARY KEY CLUSTERED ([RecipeIngredientsId] ASC),
	FOREIGN KEY ([RecipeId]) REFERENCES [dbo].[Recipes]([RecipeId])
)
