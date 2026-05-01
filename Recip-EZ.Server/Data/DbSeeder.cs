using CsvHelper;
using Microsoft.IdentityModel.Tokens;
using Recip_EZ.Server.Enums;
using Recip_EZ.Server.Models;
using System.Net.WebSockets;
using System.Globalization;
using CsvHelper.Configuration;

namespace Recip_EZ.Server.Data
{
    public class RecipeCsvMap : ClassMap<RecipeCsv>
    {
        public RecipeCsvMap()
        {
            Map(m => m.Id).Index(0);
            Map(m => m.Title).Name("title");
            Map(m => m.Ingredients).Name("ingredients");
            Map(m => m.Directions).Name("directions");
            Map(m => m.Link).Name("link");
            Map(m => m.Source).Name("source");
            Map(m => m.NER).Name("NER");
        }
    }

    public class RecipeCsv
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Ingredients { get; set; }
        public string Directions { get; set; }
        public string Link { get; set; }
        public string Source { get; set; }
        public string NER { get; set; }
    }


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
            SeedUserInventory();
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
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.ToLower()
                };

                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<RecipeCsvMap>();
                    var batch = new List<Recipe>();
                
                    foreach(var record in csv.GetRecords<RecipeCsv>())
                    {
                        var recipe = ParseRecipe(record);
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
        }

        private void SeedUserInventory()
        {
            if (_context.UserInventories.Any())
            {
                return;
            }

            using(var reader = new StreamReader("Data/Dataset_CSV/userinventory.csv"))
            {
                reader.ReadLine();

                while (!reader.EndOfStream) 
                {
                    var line = reader.ReadLine();
                    var inventoryItem = ParseUserInventory(line);

                    _context.UserInventories.Add(inventoryItem);
                }

                _context.SaveChanges();
                return;
            }
        }

        private Recipe ParseRecipe(RecipeCsv r)
        {
            var recipe = new Recipe()
            {
                RecipeName = r.Title,
                Ingredients = r.Ingredients,
                Instructions = r.Directions,
                URL = r.Link,
                Source = r.Source,
                RawIngredientList = r.NER
            };
            return recipe;
        }

        private UserInventory ParseUserInventory(string line)
        {
            var cols = line.Split(',');


            return new UserInventory()
            {
                UserId = Int32.Parse(cols[1]),
                IngredientId = Int32.Parse(cols[2]),
                Unit = (Unit)(int.Parse(cols[3])),
                Quantity = Double.Parse(cols[4]),
                DateAdded = DateTime.Parse(cols[5]),
                ExpirationDate = string.IsNullOrEmpty(cols[6]) ? null : DateTime.Parse(cols[6])
            };
        }
    }
}
