CREATE TABLE [dbo].[Ingredients] (
    [IngredientId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Ingredients] PRIMARY KEY CLUSTERED ([IngredientId] ASC)
);
