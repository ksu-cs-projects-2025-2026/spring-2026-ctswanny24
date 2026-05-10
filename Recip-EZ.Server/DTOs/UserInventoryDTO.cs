using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.DTOs
{
    public class UserInventoryDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user's inventory record.
        /// </summary>
        public int UserInventoryId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the ingredient.
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the ingredient.
        /// </summary>
        public string IngredientName { get; set; }

        /// <summary>
        /// Gets or sets the unit of measurement associated with the value.
        /// </summary>
        public Unit Unit { get; set; }

        /// <summary>
        /// Gets or sets the quantity associated with the current instance.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Gets the date and time when the item was added.
        /// </summary>
        public DateTime DateAdded { get; init; }

        /// <summary>
        /// Gets or sets the expiration date and time for the item.
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

    }
}
