using Microsoft.EntityFrameworkCore;
using Recip_EZ.Server.Data;

namespace Recip_EZ.Tests.TestSupport;

internal static class TestDbContextFactory
{
    public static RecipEzDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<RecipEzDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new RecipEzDbContext(options);
    }
}
