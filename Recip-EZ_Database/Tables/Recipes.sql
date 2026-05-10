CREATE TABLE [dbo].[Recipes] (
    [RecipeId]          INT            IDENTITY (1, 1) NOT NULL,
    [RecipeName]        NVARCHAR (MAX) NULL,
    [Ingredients]       NVARCHAR (MAX) NULL,
    [Instructions]      NVARCHAR (MAX) NULL,
    [URL]               NVARCHAR (MAX) NULL,
    [Source]            NVARCHAR (MAX) NULL,
    [RawIngredientList] NVARCHAR (MAX) NULL,
    [CanonIngredients]  NVARCHAR (MAX) NULL,
    [Priorities]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Recipes] PRIMARY KEY CLUSTERED ([RecipeId] ASC)
);

