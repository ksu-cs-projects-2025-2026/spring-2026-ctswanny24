using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.Models
{
    public class Ingredient
    {
        /// <summary>
        /// Database ID for the ingredient.
        /// </summary>
        public int IngredientId { get; init; }

        /// <summary>
        /// Name of the ingredient.
        /// </summary>
        public string? Name { get; set; }

    }
}

