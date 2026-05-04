using Microsoft.AspNetCore.Mvc;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using Recip_EZ.Tests.TestSupport;

namespace Recip_EZ.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public void GetAllUsers_WhenDatabaseIsEmpty_ReturnsEmptyList()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new UserService(context);

        var users = service.GetAllUsers();

        Assert.Empty(users);
    }

    [Fact]
    public void GetUser_ExactUsernameAndPasswordMatch_ReturnsUser()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Users.Add(new User
        {
            UserId = 3,
            Username = "caden",
            Password = "pass123",
            FirstName = "Caden",
            LastName = "Tester",
            CreatedOn = DateTime.UtcNow
        });
        context.SaveChanges();
        var service = new UserService(context);

        var user = service.GetUser("caden", "pass123");

        Assert.Equal(3, user.UserId);
        Assert.Equal("caden", user.Username);
    }

    [Theory]
    [InlineData("caden", "wrong")]
    [InlineData("Caden", "pass123")]
    [InlineData("", "")]
    public void GetUser_InvalidCredentials_Throws(string username, string password)
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Users.Add(new User
        {
            UserId = 3,
            Username = "caden",
            Password = "pass123",
            FirstName = "Caden",
            LastName = "Tester",
            CreatedOn = DateTime.UtcNow
        });
        context.SaveChanges();
        var service = new UserService(context);

        var exception = Assert.Throws<Exception>(() => service.GetUser(username, password));

        Assert.Contains($"User with username {username} not found.", exception.Message);
    }

    [Fact]
    public void AuthenticateUser_ReturnsSuccessfulJsonResult()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new UserService(context);

        var result = service.AuthenticateUser("anyone", "anything");

        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }
}
