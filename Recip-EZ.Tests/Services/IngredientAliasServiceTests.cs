using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using Recip_EZ.Tests.TestSupport;

namespace Recip_EZ.Tests.Services;

public class IngredientAliasServiceTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetAliasesForIngredientName_BlankInput_ReturnsEmptySet(string? ingredientName)
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new IngredientAliasService(context);

        var aliases = service.GetAliasesForIngredientName(ingredientName);

        Assert.Empty(aliases);
    }

    [Fact]
    public void GetAliasesForIngredientName_RemovesNoiseWordsPluralizationAndPunctuation()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new IngredientAliasService(context);

        var aliases = service.GetAliasesForIngredientName("  2 Large, diced TOMATOES!! ");

        Assert.Contains("large diced tomatoes", aliases);
        Assert.Contains("tomato", aliases);
        Assert.DoesNotContain("large diced tomato", aliases);
    }

    [Theory]
    [InlineData("berries", "berry")]
    [InlineData("potatoes", "potato")]
    [InlineData("boxes", "box")]
    [InlineData("glass", "glass")]
    public void NormalizeIngredient_SingularizesCommonPluralEndings(string ingredientName, string expected)
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new IngredientAliasService(context);

        var normalized = service.NormalizeIngredient(ingredientName);

        Assert.Equal(expected, normalized);
    }

    [Fact]
    public void AddToAliases_CreatesAliasesForIngredientsAndIsIdempotent()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Ingredients.AddRange(
            new Ingredient { IngredientId = 1, Name = "Chicken Breasts" },
            new Ingredient { IngredientId = 2, Name = "Fresh Tomatoes" },
            new Ingredient { IngredientId = 3, Name = "   " });
        context.IngredientAliases.Add(new IngredientAlias
        {
            IngredientId = 1,
            AliasName = "chicken breast"
        });
        context.SaveChanges();
        var service = new IngredientAliasService(context);

        service.AddToAliases();
        var aliasesAfterFirstRun = context.IngredientAliases.ToList();
        service.AddToAliases();
        var aliasesAfterSecondRun = context.IngredientAliases.ToList();

        Assert.Contains(aliasesAfterFirstRun, alias => alias.IngredientId == 1 && alias.AliasName == "chicken breasts");
        Assert.Contains(aliasesAfterFirstRun, alias => alias.IngredientId == 1 && alias.AliasName == "chicken breast");
        Assert.Contains(aliasesAfterFirstRun, alias => alias.IngredientId == 2 && alias.AliasName == "tomato");
        Assert.DoesNotContain(aliasesAfterFirstRun, alias => alias.IngredientId == 3);
        Assert.Equal(aliasesAfterFirstRun.Count, aliasesAfterSecondRun.Count);
    }
}
