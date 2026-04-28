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
            if (_context.Users.Any())
            {
                return;
            }

            var lines = File.ReadAllLines("Data/Dataset_CSV/userlogin.csv").Skip(1);
            foreach (var line in lines)
            {
                var cols = line.Split(',');
                var user = new User()
                { 
                    Username = cols[1],
                    Password = cols[2],
                    FirstName = cols[3],
                    LastName = cols[4],
                    CreatedOn = DateTime.Parse(cols[5]),
                };
                _context.Users.Add(user);
            }
        }

        private void SeedIngredients()
        {
            if (_context.Ingredients.Any()) return;

            string fileName;

            for (int i = 1; i <= 5; i++)
            {
                fileName = $"Data/Dataset_CSV/FOOD-DATA-GROUP{i}.csv";
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
            using (var reader = new StreamReader("Data/Dataset_CSV/recipes.csv"))
            {
                //Just so the format line is not used.
                reader.ReadLine();

                var batch = new List<Recipe>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var recipe = Parse(line);

                    batch.Add(recipe);

                    if(batch.Count >= 1500)
                    {
                        _context.Recipes.AddRange(batch);
                        _context.SaveChanges();
                        break;
                        //batch.Clear();
                    }
                }
            }
        }

        private Recipe Parse(string line)
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
            return recipe;
        }
    }
}
