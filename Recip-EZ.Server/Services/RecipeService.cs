using Microsoft.EntityFrameworkCore;
using Recip_EZ.Server.Data;
using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Services
{
    public class RecipeService
    {
        private readonly RecipEzDbContext _context;

        public RecipeService(RecipEzDbContext context)
        {
            _context = context;
        }

        public List<Recipe> GetFirstFiveRecipes()
        {
            var recipe = _context.Recipes.Take(10).ToList();

            if (!recipe.Any())
            {
                throw new Exception("No recipes found in the database.");
            }
            else
            {
                return recipe;
            }
        }

    }
}
