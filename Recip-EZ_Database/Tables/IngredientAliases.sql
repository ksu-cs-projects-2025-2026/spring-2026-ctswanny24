CREATE TABLE [dbo].[IngredientAliases] (
    [IngredientAliasId] INT            IDENTITY (1, 1) NOT NULL,
    [IngredientId]      INT            NOT NULL,
    [AliasName]         NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_IngredientAliases] PRIMARY KEY CLUSTERED ([IngredientAliasId] ASC)
);
