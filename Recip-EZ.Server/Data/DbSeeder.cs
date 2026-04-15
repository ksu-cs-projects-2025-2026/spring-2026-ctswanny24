using Recip_EZ.Server.Models;
using System.Net.WebSockets;

namespace Recip_EZ.Server.Data
{
    public class DbSeeder
    {
        private readonly RecipEzDbContext _context;

        public DbSeeder(RecipEzDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            SeedUsers();
            SeedIngredients();
            SeedRecipes();
        }

        private void SeedUsers()
        {
            if(_context.Users.Any())
            {
                return;
            }

            var lines = File.ReadAllLines("");
            foreach(var line in lines)
            {
                var cols = line.Split(',');
                var user = new User()
                {
                    Username = cols[0],
                    Password = cols[1],
                    FirstName = cols[2],
                    LastName = cols[3],
                    CreatedOn = DateTime.Now
                };
                _context.Users.Add(user);
            }
        }

        private void SeedIngredients()
        {
            if (_context.Ingredients.Any()) return;

            string fileName = "Data/Dataset_CSV/FOOD-DATA-GROUP";

            for(int i = 0; i < 5; i++)
            {
                fileName += $"{i}.csv";
                var lines = File.ReadAllLines(fileName).Skip(1); // Skip header line

                foreach (var line in lines)
                {
                    var cols = line.Split(',');

                    var ingredient = new Ingredient()
                    {
                        Name = cols[2]
                    };

                    _context.Ingredients.Add(ingredient);
                }

            }

            _context.SaveChanges();
        }

        private void SeedRecipes()
        {
            if (_context.Recipes.Any())
            {
                return;
            }
            var lines = File.ReadAllLines("Data/Dataset_CSV/recipes.csv");
            foreach (var line in lines)
            {
                var cols = line.Split(',');
                var recipe = new Recipe()
                {
                    RecipeName = cols[0],
                    Ingredients = cols[1],
                    Instructions = cols[2],
                    URL = cols[3],
                    Source = cols[4],
                    RawIngredientList = cols[5]
                };
                _context.Recipes.Add(recipe);
            }
            _context.SaveChanges();
        }
    }
}
