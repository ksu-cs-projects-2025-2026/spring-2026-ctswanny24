using Microsoft.AspNetCore.Mvc;
using Recip_EZ.Server.Controllers;
using Recip_EZ.Server.Enums;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using Recip_EZ.Tests.TestSupport;

namespace Recip_EZ.Tests.Controllers;

public class ControllerTests
{
    [Fact]
    public void InventoryController_AddIngredient_ValidPayload_ReturnsOkWithInventoryDto()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Ingredients.Add(new Ingredient { IngredientId = 1, Name = "Flour" });
        context.SaveChanges();
        var controller = new InventoryController(new InventoryService(context));

        var result = controller.AddIngredient(new InventoryPayload
        {
            UserId = 5,
            IngredientId = 1,
            Quantity = 4,
            Unit = nameof(Unit.Cup)
        });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<InventoryResponse>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Item added to inventory", response.Message);
        Assert.NotNull(response.Inventory);
        Assert.Equal("Flour", response.Inventory.Single().IngredientName);
    }

    [Fact]
    public void InventoryController_AddIngredient_InvalidUnit_ThrowsArgumentException()
    {
        using var context = TestDbContextFactory.CreateContext();
        var controller = new InventoryController(new InventoryService(context));

        Assert.Throws<ArgumentException>(() => controller.AddIngredient(new InventoryPayload
        {
            UserId = 5,
            IngredientId = 1,
            Quantity = 4,
            Unit = "NotAUnit"
        }));
    }

    [Fact]
    public void InventoryController_DeleteMissingInventoryItem_ReturnsNotFound()
    {
        using var context = TestDbContextFactory.CreateContext();
        var controller = new InventoryController(new InventoryService(context));

        var result = controller.DeleteInventoryItem(404);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void RecipeController_FetchCuratedRecipes_ReturnsOkResponseEvenWhenNoMatchesExist()
    {
        using var context = TestDbContextFactory.CreateContext();
        var aliasService = new IngredientAliasService(context);
        var controller = new RecipeController(new RecipeService(context, aliasService));

        var result = controller.FetchCuratedRecipes(userId: 77);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CuratedRecipesResponse>(okResult.Value);
        Assert.True(response.Success);
        Assert.Empty(response.Recipes);
        Assert.Contains("No recipe matches found", response.Message);
    }

    [Fact]
    public void LoginController_Login_ValidCredentials_ReturnsOk()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Users.Add(new User
        {
            UserId = 12,
            Username = "demo",
            Password = "secret",
            FirstName = "Demo",
            LastName = "User",
            CreatedOn = DateTime.UtcNow
        });
        context.SaveChanges();
        var controller = new LoginController(new UserService(context));

        var result = controller.Login(new LoginRequest { Username = "demo", Password = "secret" });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(12, response.UserId);
    }

    [Fact]
    public void LoginController_Login_InvalidCredentials_PropagatesServiceException()
    {
        using var context = TestDbContextFactory.CreateContext();
        var controller = new LoginController(new UserService(context));

        Assert.Throws<Exception>(() => controller.Login(new LoginRequest
        {
            Username = "missing",
            Password = "wrong"
        }));
    }
}
