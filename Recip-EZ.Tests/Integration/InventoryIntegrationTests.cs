using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Recip_EZ.Server.Controllers;
using Recip_EZ.Server.Data;
using Recip_EZ.Server.Enums;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using Recip_EZ.Tests.TestSupport;
using System.Security.Claims;

namespace Recip_EZ.Tests.Integration;

public class InventoryIntegrationTests
{
    [Fact]
    public void AddIngredient_ThenFetchInventory_ReturnsPersistedIngredientForAuthenticatedUser()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Ingredients.Add(new Ingredient { IngredientId = 1, Name = "Cheddar Cheese" });
        context.SaveChanges();
        var controller = CreateInventoryController(context, userId: 7);

        var addResult = controller.AddIngredient(new InventoryPayload
        {
            IngredientId = 1,
            Quantity = 2,
            Unit = nameof(Unit.Cup)
        });
        var fetchResult = controller.FetchUserInventory();

        Assert.IsType<OkObjectResult>(addResult);
        var okResult = Assert.IsType<OkObjectResult>(fetchResult);
        var response = Assert.IsType<InventoryResponse>(okResult.Value);
        var item = Assert.Single(response.Inventory!);
        Assert.True(response.Success);
        Assert.Equal(1, item.IngredientId);
        Assert.Equal("Cheddar Cheese", item.IngredientName);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(Unit.Cup, item.Unit);
    }

    [Fact]
    public void FetchUserInventory_ReturnsOnlyItemsOwnedByAuthenticatedUser()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Ingredients.AddRange(
            new Ingredient { IngredientId = 1, Name = "Eggs" },
            new Ingredient { IngredientId = 2, Name = "Milk" });
        context.UserInventories.AddRange(
            new UserInventory { UserInventoryId = 101, UserId = 12, IngredientId = 1, Quantity = 12, Unit = Unit.Item },
            new UserInventory { UserInventoryId = 102, UserId = 44, IngredientId = 2, Quantity = 1, Unit = Unit.Gallon });
        context.SaveChanges();
        var controller = CreateInventoryController(context, userId: 12);

        var result = controller.FetchUserInventory();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<InventoryResponse>(okResult.Value);
        var item = Assert.Single(response.Inventory!);
        Assert.True(response.Success);
        Assert.Equal(101, item.UserInventoryId);
        Assert.Equal("Eggs", item.IngredientName);
    }

    [Fact]
    public void DeleteInventoryItem_RemovesOnlyMatchingAuthenticatedUsersItem()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Ingredients.Add(new Ingredient { IngredientId = 1, Name = "Flour" });
        context.UserInventories.AddRange(
            new UserInventory { UserInventoryId = 201, UserId = 5, IngredientId = 1, Quantity = 4, Unit = Unit.Cup },
            new UserInventory { UserInventoryId = 202, UserId = 9, IngredientId = 1, Quantity = 8, Unit = Unit.Cup });
        context.SaveChanges();
        var controller = CreateInventoryController(context, userId: 5);

        var deleteResult = controller.DeleteInventoryItem(201);
        var fetchResult = controller.FetchUserInventory();

        Assert.IsType<OkObjectResult>(deleteResult);
        var okResult = Assert.IsType<OkObjectResult>(fetchResult);
        var response = Assert.IsType<InventoryResponse>(okResult.Value);
        Assert.True(response.Success);
        Assert.Empty(response.Inventory!);
        Assert.Single(context.UserInventories);
        Assert.Equal(202, context.UserInventories.Single().UserInventoryId);
    }

    [Fact]
    public void DeleteInventoryItem_ForAnotherUsersItem_ReturnsNotFoundAndKeepsItem()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.UserInventories.Add(new UserInventory
        {
            UserInventoryId = 301,
            UserId = 22,
            IngredientId = 1,
            Quantity = 1,
            Unit = Unit.Item
        });
        context.SaveChanges();
        var controller = CreateInventoryController(context, userId: 99);

        var result = controller.DeleteInventoryItem(301);

        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Single(context.UserInventories);
    }

    private static InventoryController CreateInventoryController(RecipEzDbContext context, int userId)
    {
        return new InventoryController(new InventoryService(context))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }, "TestAuth"))
                }
            }
        };
    }
}
