CREATE TABLE [dbo].[Recipes]
(
	[RecipeId] INT IDENTITY(1,1) NOT NULL,
	[RecipeName] NVARCHAR(255) NOT NULL,
	[Instructions] NVARCHAR(MAX) NOT NULL,
	
	[URL] NVARCHAR(255) NULL,
	[Source] NVARCHAR(255) NULL,
	[Description] NVARCHAR(MAX) NULL,
	[Difficulty] INT NULL,
	--[Cuisine] NVARCHAR(50) NULL,
	[PreparationTime] INT NULL,

	PRIMARY KEY CLUSTERED ([RecipeId] ASC),
)
