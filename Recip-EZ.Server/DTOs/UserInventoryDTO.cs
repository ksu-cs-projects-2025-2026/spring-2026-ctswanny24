using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.DTOs
{
    public class UserInventoryDTO
    {
        public int UserInventoryId { get; set; }

        public int UserId { get; set; }

        public int IngredientId { get; set; }

        public string IngredientName { get; set; }

        public Unit Unit { get; set; }

        public double Quantity { get; set; }

        public DateTime DateAdded { get; init; }

        public DateTime? ExpirationDate { get; set; }

    }
}
