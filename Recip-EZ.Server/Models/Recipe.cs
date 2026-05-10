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

        /// <summary>
        /// Gets the list of ingredients associated with the recipe. This includes a string of quantity, unit, and ingredient name for each ingredient in the recipe. This is the raw ingredient list that is obtained from the recipe source and may not be standardized or normalized in any way. It is used for display purposes.
        /// </summary>
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

        /// <summary>
        /// List of the ingredients in the recipe, standardized to a canonical form. This is used for matching against inventory items and for display purposes.
        /// </summary>
        public string? CanonIngredients { get; init; }

        /// <summary>
        /// List of priorities associated with the current recipe. Each index of priority matches with the index of the ingredient item in CanonIngredients
        /// </summary>
        public string? Priorities { get; init; }
        
    }
}
