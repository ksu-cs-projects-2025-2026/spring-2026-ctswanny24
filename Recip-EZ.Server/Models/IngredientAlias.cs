namespace Recip_EZ.Server.Models
{
    public class IngredientAlias
    {
        /// <summary>
        /// DB id for ingredient alias. Primary key.
        /// </summary>
        public int IngredientAliasId { get; set; }

        /// <summary>
        /// IngredientId that this alias is associated with. Foreign key to Ingredients table.
        /// </summary>
        public required int IngredientId { get; set; }

        /// <summary>
        /// String representation of the alias name. This is the name that will be used in the search and matching process for recipes. 
        /// It is important to note that this is not necessarily the same as the original ingredient name, as it may have been normalized or modified in some way to improve searchability and matching accuracy.
        /// </summary>
        public required string AliasName { get; set; }
    }
}
