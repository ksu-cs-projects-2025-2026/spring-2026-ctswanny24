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
    public void GetUser_InvalidCredentials_ReturnsNull(string username, string password)
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

        var user = service.GetUser(username, password);

        Assert.Null(user);
    }
}
