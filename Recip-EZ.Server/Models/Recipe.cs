using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Models
{
    /// <summary>
    /// Model representing a recipe.
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// Id of the recipe in the database
        /// </summary>
        public int RecipeId { get; init; }

        /// <summary>
        /// Name of the recipe
        /// </summary>
        public string? RecipeName { get; init; }

        public string? Ingredients { get; init; }

        /// <summary>
        /// Instructions for the recipe
        /// </summary>
        public string? Instructions { get; init; }

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
        public string? RawIngredientList { get; init; }

        public string? CanonIngredients { get; init; }

        public string? Priorities { get; init; }
        
    }
}
