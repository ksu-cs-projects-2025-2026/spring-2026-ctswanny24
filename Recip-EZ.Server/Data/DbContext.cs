using Microsoft.EntityFrameworkCore;
using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Data
{
    public class DbContext
    {
        public DbContext(DbContextOptions<DbContext> options)
                : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Recipe> Recipes { get; set; } 
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<SavedRecipe> SavedRecipes { get; set; }
    }
}
