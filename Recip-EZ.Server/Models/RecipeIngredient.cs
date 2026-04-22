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

        public int IngredientId { get; init; }
        
    }
}
