using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.Models
{
    public class CanonicalIngredient
    {
        public int CanonicalIngredientId { get; set; }
        public int? IngredientId { get; set; } = null;
        public string RawText { get; set; }
        public string CleanedText { get; set; }
        public string CanonicalName { get; set; }
        public double quantity { get; set; }
        public Unit? Unit { get; set; }
        public string Preparation { get; set; }
        public Priority Importance { get; set; }
        public double Confidence { get; set; }
    }
}
