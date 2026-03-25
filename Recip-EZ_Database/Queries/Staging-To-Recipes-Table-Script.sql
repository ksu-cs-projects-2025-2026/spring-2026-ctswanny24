INSERT INTO dbo.Recipes (RecipeName, Ingredients, Instructions, URL, Source, RawIngredientList)
SELECT RecipeName, Ingredients, Instructions, URL, Source, RawIngredientList
FROM dbo.Recipes_Staging
WHERE NOT EXISTS(
    SELECT 1
    FROM dbo.Recipes r
    WHERE r.RecipeName = dbo.Recipes_Staging.RecipeName
)