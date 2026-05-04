using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using Recip_EZ.Tests.TestSupport;

namespace Recip_EZ.Tests.Services;

public class RecipeServiceTests
{
    [Fact]
    public void GetAllRecipes_WhenNoRecipesExist_Throws()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = CreateService(context);

        var exception = Assert.Throws<Exception>(() => service.GetAllRecipes());

        Assert.Equal("No Recipes found in the database.", exception.Message);
    }

    [Fact]
    public void GetNRecipes_WhenNumIsLessThanAvailable_ReturnsThatManyRecipes()
    {
        using var context = TestDbContextFactory.CreateContext();
        SeedRecipes(context);
        var service = CreateService(context);

        var recipes = service.GetNRecipes(2);

        Assert.Equal(2, recipes.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void GetNRecipes_NonPositiveNum_ThrowsBecauseNoRecipesAreSelected(int num)
    {
        using var context = TestDbContextFactory.CreateContext();
        SeedRecipes(context);
        var service = CreateService(context);

        var exception = Assert.Throws<Exception>(() => service.GetNRecipes(num));

        Assert.Equal("No Recipes found in the database.", exception.Message);
    }

    [Fact]
    public void GetRecipeById_ExistingRecipe_ReturnsRecipe()
    {
        using var context = TestDbContextFactory.CreateContext();
        SeedRecipes(context);
        var service = CreateService(context);

        var recipe = service.GetRecipeById(2);

        Assert.Equal("Tomato Soup", recipe.RecipeName);
    }

    [Fact]
    public void GetRecipeById_MissingRecipe_ThrowsWithId()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = CreateService(context);

        var exception = Assert.Throws<Exception>(() => service.GetRecipeById(99));

        Assert.Equal("No Recipe found with ID: 99", exception.Message);
    }

    [Fact]
    public void ToDTO_NullJsonFields_ReturnsEmptyListsAndStrings()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = CreateService(context);
        var recipe = new Recipe { RecipeId = 12 };

        var dto = service.ToDTO(recipe);

        Assert.Equal(12, dto.RecipeId);
        Assert.Equal(string.Empty, dto.RecipeName);
        Assert.Empty(dto.Ingredients);
        Assert.Empty(dto.Instructions);
        Assert.Empty(dto.RawIngredientList);
        Assert.Equal(string.Empty, dto.URL);
        Assert.Equal(string.Empty, dto.Source);
    }

    [Fact]
    public void ToDTO_ValidJsonFields_DeserializesLists()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = CreateService(context);
        var recipe = new Recipe
        {
            RecipeId = 12,
            RecipeName = "Toast",
            Ingredients = "[\"bread\",\"butter\"]",
            Instructions = "[\"toast bread\",\"spread butter\"]",
            RawIngredientList = "[\"bread\",\"butter\"]",
            URL = "https://example.test/toast",
            Source = "Example"
        };

        var dto = service.ToDTO(recipe);

        Assert.Equal(new[] { "bread", "butter" }, dto.Ingredients);
        Assert.Equal(new[] { "toast bread", "spread butter" }, dto.Instructions);
        Assert.Equal(new[] { "bread", "butter" }, dto.RawIngredientList);
        Assert.Equal("https://example.test/toast", dto.URL);
        Assert.Equal("Example", dto.Source);
    }

    [Fact]
    public void ToDTO_InvalidJson_ThrowsJsonException()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = CreateService(context);
        var recipe = new Recipe { RecipeId = 12, Ingredients = "not json" };

        Assert.Throws<System.Text.Json.JsonException>(() => service.ToDTO(recipe));
    }

    [Fact]
    public void GetCuratedRecipesForUser_UserWithoutInventory_ReturnsEmptyList()
    {
        using var context = TestDbContextFactory.CreateContext();
        SeedRecipes(context);
        var service = CreateService(context);

        var recipes = service.GetCuratedRecipesForUser(userId: 999);

        Assert.Empty(recipes);
    }

    [Fact]
    public void GetCuratedRecipesForUser_RanksCompleteMatchesBeforePartialMatches()
    {
        using var context = TestDbContextFactory.CreateContext();
        SeedInventoryForUser(context, userId: 7);
        SeedRecipes(context);
        var service = CreateService(context);

        var recipes = service.GetCuratedRecipesForUser(userId: 7, limit: 10);

        Assert.Equal(2, recipes.Count);
        Assert.Equal("Chicken Dinner", recipes[0].RecipeName);
        Assert.True(recipes[0].CanMakeNow);
        Assert.Equal(100, recipes[0].MatchPercentage);
        Assert.Equal("Tomato Soup", recipes[1].RecipeName);
        Assert.False(recipes[1].CanMakeNow);
        Assert.Equal(50, recipes[1].MatchPercentage);
    }

    [Fact]
    public void GetCuratedRecipesForUser_ClampsMinimumMatchPercentageAndFiltersWeakMatches()
    {
        using var context = TestDbContextFactory.CreateContext();
        SeedInventoryForUser(context, userId: 7);
        SeedRecipes(context);
        var service = CreateService(context);

        var highThresholdRecipes = service.GetCuratedRecipesForUser(userId: 7, minimumMatchPercentage: 75);
        var aboveMaxThresholdRecipes = service.GetCuratedRecipesForUser(userId: 7, minimumMatchPercentage: 1000);

        var recipe = Assert.Single(highThresholdRecipes);
        Assert.Equal("Chicken Dinner", recipe.RecipeName);
        Assert.Single(aboveMaxThresholdRecipes);
    }

    [Fact]
    public void GetCuratedRecipesForUser_NonPositiveLimit_DefaultsToTwentyFive()
    {
        using var context = TestDbContextFactory.CreateContext();
        SeedInventoryForUser(context, userId: 7);
        SeedRecipes(context);
        var service = CreateService(context);

        var recipes = service.GetCuratedRecipesForUser(userId: 7, limit: 0);

        Assert.Equal(2, recipes.Count);
    }

    private static RecipeService CreateService(Recip_EZ.Server.Data.RecipEzDbContext context)
    {
        var aliasService = new IngredientAliasService(context);
        return new RecipeService(context, aliasService);
    }

    private static void SeedInventoryForUser(Recip_EZ.Server.Data.RecipEzDbContext context, int userId)
    {
        context.Ingredients.AddRange(
            new Ingredient { IngredientId = 1, Name = "Chicken Breasts" },
            new Ingredient { IngredientId = 2, Name = "Rice" },
            new Ingredient { IngredientId = 3, Name = "Tomatoes" });
        context.UserInventories.AddRange(
            new UserInventory { UserInventoryId = 1, UserId = userId, IngredientId = 1, Quantity = 2 },
            new UserInventory { UserInventoryId = 2, UserId = userId, IngredientId = 2, Quantity = 1 },
            new UserInventory { UserInventoryId = 3, UserId = userId, IngredientId = 3, Quantity = 3 });
        context.SaveChanges();
    }

    private static void SeedRecipes(Recip_EZ.Server.Data.RecipEzDbContext context)
    {
        context.Recipes.AddRange(
            new Recipe
            {
                RecipeId = 1,
                RecipeName = "Chicken Dinner",
                Ingredients = "[\"Chicken Breasts\",\"Rice\"]",
                Instructions = "[\"Cook chicken\",\"Serve with rice\"]",
                RawIngredientList = "[\"boneless chicken breasts\",\"rice\"]"
            },
            new Recipe
            {
                RecipeId = 2,
                RecipeName = "Tomato Soup",
                Ingredients = "[\"Tomatoes\",\"Cream\"]",
                Instructions = "[\"Simmer\"]",
                RawIngredientList = "[\"tomatoes\",\"cream\"]"
            },
            new Recipe
            {
                RecipeId = 3,
                RecipeName = "Air Pie",
                Ingredients = "[]",
                Instructions = "[]",
                RawIngredientList = "[]"
            });
        context.SaveChanges();
    }
}
