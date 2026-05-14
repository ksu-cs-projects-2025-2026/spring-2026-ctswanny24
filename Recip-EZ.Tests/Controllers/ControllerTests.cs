using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Recip_EZ.Server.Controllers;
using Recip_EZ.Server.Enums;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using Recip_EZ.Tests.TestSupport;
using System.Security.Claims;

namespace Recip_EZ.Tests.Controllers;

public class ControllerTests
{
    [Fact]
    public void InventoryController_AddIngredient_ValidPayload_ReturnsOkWithInventoryDto()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Ingredients.Add(new Ingredient { IngredientId = 1, Name = "Flour" });
        context.SaveChanges();
        var controller = CreateInventoryController(context, userId: 5);

        var result = controller.AddIngredient(new InventoryPayload
        {
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
        var controller = CreateInventoryController(context, userId: 5);

        Assert.Throws<ArgumentException>(() => controller.AddIngredient(new InventoryPayload
        {
            IngredientId = 1,
            Quantity = 4,
            Unit = "NotAUnit"
        }));
    }

    [Fact]
    public void InventoryController_DeleteMissingInventoryItem_ReturnsNotFound()
    {
        using var context = TestDbContextFactory.CreateContext();
        var controller = CreateInventoryController(context, userId: 5);

        var result = controller.DeleteInventoryItem(404);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void RecipeController_FetchCuratedRecipes_ReturnsOkResponseEvenWhenNoMatchesExist()
    {
        using var context = TestDbContextFactory.CreateContext();
        var aliasService = new IngredientAliasService(context);
        var matchingService = new MatchingService(context, aliasService);
        var controller = new RecipeController(new RecipeService(context, aliasService, matchingService))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "77")
                    }, "TestAuth"))
                }
            }
        };

        var result = controller.FetchCuratedRecipes();

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
        var controller = new LoginController(new UserService(context), CreateJwtConfig())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = controller.Login(new LoginRequest { Username = "demo", Password = "secret" });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(12, response.UserId);
    }

    [Fact]
    public void LoginController_Login_InvalidCredentials_ReturnsUnauthorized()
    {
        using var context = TestDbContextFactory.CreateContext();
        var controller = new LoginController(new UserService(context), CreateJwtConfig())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = controller.Login(new LoginRequest
        {
            Username = "missing",
            Password = "wrong"
        });

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(unauthorizedResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Check your username and password and try again.", response.Message);
    }

    [Fact]
    public void LoginController_Register_ValidPayload_ReturnsCreated()
    {
        using var context = TestDbContextFactory.CreateContext();
        var controller = new LoginController(new UserService(context), CreateJwtConfig())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = controller.Register(new RegisterRequest
        {
            Username = "new@example.com",
            Password = "secret",
            FirstName = "New",
            LastName = "User"
        });

        var createdResult = Assert.IsType<CreatedResult>(result);
        var response = Assert.IsType<LoginResponse>(createdResult.Value);
        Assert.True(response.Success);
        Assert.Single(context.Users);
    }

    [Fact]
    public void LoginController_Register_DuplicateUsername_ReturnsConflict()
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
        var controller = new LoginController(new UserService(context), CreateJwtConfig())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = controller.Register(new RegisterRequest
        {
            Username = "demo",
            Password = "secret",
            FirstName = "Other",
            LastName = "User"
        });

        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(conflictResult.Value);
        Assert.False(response.Success);
        Assert.Equal("An account with that email already exists.", response.Message);
    }

    private static InventoryController CreateInventoryController(Recip_EZ.Server.Data.RecipEzDbContext context, int userId)
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

    private static IConfiguration CreateJwtConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "unit-test-secret-key-that-is-long-enough",
                ["Jwt:Issuer"] = "RecipEZ.Tests",
                ["Jwt:Audience"] = "RecipEZ.Tests",
                ["Jwt:DurationInMinutes"] = "60"
            })
            .Build();
    }
}
