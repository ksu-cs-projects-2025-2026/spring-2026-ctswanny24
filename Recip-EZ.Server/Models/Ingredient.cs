using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.Models
{
    public class Ingredient
    {
        /// <summary>
        /// Database ID for the ingredient.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Name of the ingredient.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unit of measurement used for this ingredient
        /// </summary>
        public Unit Unit { get; set; }

        /// <summary>
        /// Categorical type of the ingredient.
        /// </summary>
        public IngredientCategory Category { get; set; }

        /// <summary>
        /// Gets the quantity of the ingredient. To help understand the amount of the ingredient left.
        /// </summary>
        public float Quantity { get; set; }

        /// <summary>
        /// The date the ingredient was added. To help with tracking the freshness of the ingredient and when it might need to be used by or thrown out.
        /// </summary>
        public DateTime DateOfCreation { get; set; }

        /// <summary>
        /// Similar to date of creation, but this is the specific expiration date.
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

    }
}
