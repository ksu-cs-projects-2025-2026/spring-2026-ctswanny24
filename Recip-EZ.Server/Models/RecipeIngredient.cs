using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.Models
{
    public class RecipeIngredient
    {
        /// <summary>
        /// The id of the recipe Ingredient from the database.
        /// </summary>
        public int RecipeIngredientId { get; init; }

        public int RecipeId { get; init; }
        
        /// <summary>
        /// Name of the ingredient
        /// </summary>
        public string Name { get; init; }
                
        /// <summary>
        /// Quantity of the ingredient needed for the recipe.
        /// </summary>
        public double Quantity { get; init; }

        public Unit Unit { get; init; }

        /// <summary>
        /// Specific prep instructions for the ingredient. NOT REQUIRED. 
        /// This is for things like "chopped", "diced", "minced", etc. that may be needed for the recipe.
        /// </summary>
        public string? PrepInstructions { get; init; }
    }
}
