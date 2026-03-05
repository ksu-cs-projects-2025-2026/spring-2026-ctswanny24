using Recip_EZ.Server.Interfaces;
using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.Models
{
    public class RecipeIngredient : IIngredient
    {
        /// <summary>
        /// The id of the recipe Ingredient from the database.
        /// </summary>
        public int Id { get; init; }
        
        /// <summary>
        /// Name of the ingredient
        /// </summary>
        public string Name { get; init; }
        
        /// <summary>
        /// Gets the unit of measurement for the ingredient.
        /// </summary>
        public Unit Unit { get; init; }
        
        /// <summary>
        /// Quantity of the ingredient needed for the recipe.
        /// </summary>
        public float Quantity { get; init; }

        /// <summary>
        /// Specific prep instructions for the ingredient. NOT REQUIRED. 
        /// This is for things like "chopped", "diced", "minced", etc. that may be needed for the recipe.
        /// </summary>
        public string? PrepInstructions { get; init; }
    }
}
