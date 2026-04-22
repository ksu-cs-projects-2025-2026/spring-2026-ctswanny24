using Recip_EZ.Server.Data;
using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Services
{
    public class IngredientAliasService
    {
        private readonly RecipEzDbContext _context;

        public IngredientAliasService(RecipEzDbContext context)
        {
            _context = context;
        }

        public void AddToAliases()
        {
            var ingredients = _context.Ingredients.ToList();
            foreach (var i in ingredients)
            {
                var normalized = Normalize(i.Name);

                bool exists = _context.Aliases.Any(a =>
                    a.IngredientId == i.IngredientId &&
                    a.AliasName.ToLower() == normalized);

                if (!exists)
                {
                    _context.Aliases.Add(new IngredientAlias
                    {
                        IngredientId = i.IngredientId,
                        AliasName = normalized
                    });
                }

                bool originalExists = _context.Aliases.Any(a =>
                    a.IngredientId == i.IngredientId &&
                    a.AliasName.ToLower() == i.Name.ToLower());

                if (!originalExists)
                {
                    _context.Aliases.Add(new IngredientAlias
                    {
                        IngredientId = i.IngredientId,
                        AliasName = i.Name.ToLower()
                    });
                }
            }
            _context.SaveChanges();
        }

        private string Normalize(string name)
        {
            name = name.Trim().ToLower();

            name = name.Replace("raw", "")
                .Replace("cooked", "")
                .Replace("roasted", "")
                .Replace("fried", "");

            if (name.EndsWith("ies"))
                name = name.Substring(0, name.Length - 3) + "y"; // berries → berry
            else if (name.EndsWith("es"))
                name = name.Substring(0, name.Length - 2); // tomatoes → tomato (mostly works)
            else if (name.EndsWith("s") && !name.EndsWith("ss"))
                name = name.Substring(0, name.Length - 1); // eggs → egg

            return name;
        }

    }
}
