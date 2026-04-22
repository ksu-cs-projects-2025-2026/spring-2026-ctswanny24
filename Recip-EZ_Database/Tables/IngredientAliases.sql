CREATE TABLE [dbo].[IngredientAliases]
(
	[IngredientAliasId] INT NOT NULL IDENTITY PRIMARY KEY,
	[IngredientId] INT NOT NULL,
	[AliasName] NVARCHAR(255) NOT NULL,

	CONSTRAINT FK_IngredientAliases_Ingredients
		FOREIGN KEY (IngredientId)
		REFERENCES dbo.Ingredients.(IngredientId)
);
