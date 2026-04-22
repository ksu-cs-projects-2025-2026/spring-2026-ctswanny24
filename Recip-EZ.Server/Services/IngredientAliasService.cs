using Recip_EZ.Server.Data;
using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Services
{
    public class IngredientAliasService
    {
        #region Fields

        private readonly RecipEzDbContext _context;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Constructor for the service so that the DbContext can be used in the methods of this service.
        /// </summary>
        /// <param name="context">The database context to be used by the service</param>
        public IngredientAliasService(RecipEzDbContext context)
        {
            _context = context;
        }

        #endregion

        #region CRUD Methods

        /// <summary>
        /// Adds the normalized ingredient names as aliases. 
        /// This helps with matching ingredients that may have different forms (e.g., "tomatoes" vs "tomato") and removes common descriptors like "raw" or "cooked". 
        /// It also ensures the original name is included as an alias in lowercase for consistent matching.
        /// PRETTY VITAL FOR THE FUNCTIONALITY OF THE ALGORITHMIC COMPONENTS, SO BE CAREFUL WITH CHANGES TO THIS METHOD.
        /// </summary>
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

        #endregion

        #region Helper Methods

        /// <summary>
        /// Normalizing helper method to make the ingredient names more consistent for matching. 
        /// This method removes common descriptors like "raw", "cooked", "roasted", and "fried", and also handles plural forms by converting them to singular (e.g., "tomatoes" to "tomato").
        /// SUBJECT TO CHANGE A BIT FOR IMPROVEMENT OF MATCHING ALGORITHM, BUT THIS IS THE BASIC IDEA.
        /// </summary>
        /// <param name="name">Name of the item to be normalized</param>
        /// <returns>Normalized name</returns>
        private string Normalize(string name)
        {
            name = name.Trim().ToLower();

            //Used to replace the common prep descriptors. This could cause problems with matching.
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

        #endregion

    }
}
