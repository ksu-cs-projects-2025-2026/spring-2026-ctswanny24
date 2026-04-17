using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.Models
{
    public class UserInventory
    {
        public int UserInventoryId { get; set; }

        public int UserId { get; set; }

        public int IngredientId { get; set; }

        public Unit Unit { get; set; }

        public float Quantity { get; set; }

        public DateTime DateAdded { get; init; }

        public DateTime? ExpirationDate { get; set; }
    }
}
