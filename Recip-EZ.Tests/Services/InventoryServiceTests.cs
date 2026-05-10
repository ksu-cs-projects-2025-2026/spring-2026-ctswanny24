using Recip_EZ.Server.Enums;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using Recip_EZ.Tests.TestSupport;

namespace Recip_EZ.Tests.Services;

public class InventoryServiceTests
{
    [Fact]
    public void GetIngredients_WhenNoIngredientsExist_ReturnsEmptyList()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new InventoryService(context);

        var ingredients = service.GetIngredients();

        Assert.Empty(ingredients);
    }

    [Fact]
    public void AddItem_ValidInventoryItem_PersistsAndReturnsItem()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new InventoryService(context);
        var item = new UserInventory
        {
            UserId = 42,
            IngredientId = 7,
            Quantity = 2.5,
            Unit = Unit.Cup,
            DateAdded = DateTime.UtcNow
        };

        var result = service.AddItem(item);

        Assert.Same(item, result);
        Assert.Single(context.UserInventories);
        Assert.Equal(Unit.Cup, context.UserInventories.Single().Unit);
    }

    [Fact]
    public void AddItem_NullItem_ReturnsNull()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new InventoryService(context);

        var result = service.AddItem(null!);

        Assert.Null(result);
        Assert.Empty(context.UserInventories);
    }

    [Fact]
    public void GetInventory_ReturnsOnlyRequestedUsersInventoryWithIngredientNames()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Ingredients.AddRange(
            new Ingredient { IngredientId = 1, Name = "Eggs" },
            new Ingredient { IngredientId = 2, Name = "Milk" });
        context.UserInventories.AddRange(
            new UserInventory { UserInventoryId = 10, UserId = 5, IngredientId = 1, Quantity = 12, Unit = Unit.Item },
            new UserInventory { UserInventoryId = 11, UserId = 6, IngredientId = 2, Quantity = 1, Unit = Unit.Gallon });
        context.SaveChanges();
        var service = new InventoryService(context);

        var inventory = service.GetInventory(5);

        var item = Assert.Single(inventory);
        Assert.Equal(10, item.UserInventoryId);
        Assert.Equal(1, item.IngredientId);
        Assert.Equal("Eggs", item.IngredientName);
        Assert.Equal(12, item.Quantity);
        Assert.Equal(Unit.Item, item.Unit);
    }

    [Fact]
    public void GetInventory_UserWithNoInventory_ReturnsEmptyList()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new InventoryService(context);

        var inventory = service.GetInventory(999);

        Assert.Empty(inventory);
    }

    [Fact]
    public void DeleteItem_ExistingItem_RemovesIt()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.UserInventories.Add(new UserInventory
        {
            UserInventoryId = 15,
            UserId = 1,
            IngredientId = 2,
            Quantity = 1,
            Unit = Unit.Item
        });
        context.SaveChanges();
        var service = new InventoryService(context);

        service.DeleteItem(15, userId: 1);

        Assert.Empty(context.UserInventories);
    }

    [Fact]
    public void DeleteItem_MissingItem_ThrowsClearException()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new InventoryService(context);

        var exception = Assert.Throws<Exception>(() => service.DeleteItem(404, userId: 1));

        Assert.Equal("Item not found", exception.Message);
    }

    [Fact]
    public void GetIngredientName_MissingIngredient_ThrowsClearException()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new InventoryService(context);

        var exception = Assert.Throws<Exception>(() => service.GetIngredientName(404));

        Assert.Equal("Ingredient not found", exception.Message);
    }
}
