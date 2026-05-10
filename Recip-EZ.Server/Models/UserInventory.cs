using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.Models
{
    public class UserInventory
    {
        /// <summary>
        /// Main id for the row in the database. Primary key.
        /// </summary>
        public int UserInventoryId { get; set; }

        /// <summary>
        /// User id this inventory item belongs to. Foreign key to Users table.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Id of the ingredient in the inventory. Foreign key to Ingredients table.
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Unit of measurement for the ingredient quantity.
        /// </summary>
        public Unit Unit { get; set; }

        /// <summary>
        /// Quantity of ingredient in the inventory.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Date and time when the item was added to the inventory.
        /// </summary>
        public DateTime DateAdded { get; init; }

        /// <summary>
        /// Gets or sets the expiration date and time for the item.
        /// </summary>
        public DateTime? ExpirationDate { get; set; }
    }
}
