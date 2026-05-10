using Recip_EZ.Server.Models;
using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.DTOs
{
    public class RecipeDTO
    {
        /// <summary>
        /// Id of the recipe in the database
        /// </summary>
        public int RecipeId { get; init; }

        /// <summary>
        /// Name of the recipe
        /// </summary>
        public string? RecipeName { get; init; }

        public List<string>? Ingredients { get; init; }

        /// <summary>
        /// Instructions for the recipe
        /// </summary>
        public List<string>? Instructions { get; init; }

        /// <summary>
        /// URL to access the recipe online.
        /// </summary>
        public string? URL { get; init; }

        /// <summary>
        /// Source from where the recipe was obtained.
        /// </summary>
        public string? Source { get; init; }

        /// <summary>
        /// Description of the recipe.
        /// </summary>
        public List<string>? RawIngredientList { get; init; }

        /// <summary>
        /// List of the ingredients in the recipe, standardized to a canonical form. This is used for matching against inventory items and for display purposes.
        /// </summary>
        public List<string>? CanonIngredients { get; init; }

        /// <summary>
        /// Gets the collection of priorities associated with the current recipe. Each index of priority matches with the index of the ingredient item in CanonIngredients
        /// </summary>
        public List<Priority>? Priorities { get; init; }
        }
    }
