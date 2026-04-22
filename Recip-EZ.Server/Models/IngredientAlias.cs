namespace Recip_EZ.Server.Models
{
    public class IngredientAlias
    {
        public int IngredientAliasId { get; set; }
        
        public int IngredientId { get; set; }

        public string AliasName { get; set; }
    }
}
