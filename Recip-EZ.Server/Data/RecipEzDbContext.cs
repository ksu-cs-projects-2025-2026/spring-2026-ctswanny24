using Microsoft.EntityFrameworkCore;
using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Data
{
    public class RecipEzDbContext : DbContext
    {
        public RecipEzDbContext(DbContextOptions<RecipEzDbContext> options)
                : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ingredient> InventoryItems { get; set; }
        public DbSet<Recipe> Recipes { get; set; } 
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

    }
}
