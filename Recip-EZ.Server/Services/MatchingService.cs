using Recip_EZ.Server.Data;
using Recip_EZ.Server.Enums;
using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Services
{
    /// <summary>
    /// Provides methods for matching recipe ingredients to inventory items using exact and fuzzy alias-based
    /// comparison.
    /// </summary>
    /// <remarks>This service enables ingredient matching by leveraging alias information, supporting both
    /// exact and future fuzzy matching strategies. It is intended to be used in scenarios where ingredient names may
    /// vary but refer to the same underlying item, such as recipe management or inventory reconciliation
    /// systems.</remarks>
    public class MatchingService
    {

        #region Fields & Constructor
        RecipEzDbContext _context;
        IngredientAliasService _aliasService;

        public MatchingService(RecipEzDbContext context, IngredientAliasService aliasService)
        {
            _context = context;
            _aliasService = aliasService;
        }
        #endregion

        #region Matching Logic

        /// <summary>
        /// Logic to find exact matches between recipe ingredients and inventory items based on their aliases.
        /// </summary>
        /// <param name="inventoryAliases">Dictionary of ingredient aliases for the inventory items</param>
        /// <param name="recipeIngredientAliases">Dictionary of ingredient aliases for the recipe ingredients</param>
        /// <returns>List of indices of the matched recipe ingredients</returns>
        public List<int> FindExactMatch(Dictionary<int, HashSet<string>> inventoryAliases, Dictionary<int, HashSet<string>> recipeIngredientAliases)
        {
            List<int> matchedIngredientsIndexList = new List<int>();
            
            foreach(var recipeIngredient in recipeIngredientAliases) 
            {
                int recipeIngredientIndex = recipeIngredient.Key;
                HashSet<string> recipeAliases = recipeIngredient.Value;

                if(FindAliasMatch(recipeAliases, inventoryAliases))
                {
                    matchedIngredientsIndexList.Add(recipeIngredientIndex);
                }
            }

            return matchedIngredientsIndexList;
        }

        /// <summary>
        /// Determines whether any of the specified recipe aliases match any aliases in the inventory.
        /// </summary>
        /// <param name="recipeAliases">A set of alias strings to search for in the inventory. Cannot be null.</param>
        /// <param name="inventoryAliases">A dictionary mapping item identifiers to sets of alias strings present in the inventory. Cannot be null.</param>
        /// <returns>true if at least one alias in recipeAliases is found in any of the inventory alias sets; otherwise, false.</returns>
        public bool FindAliasMatch(HashSet<string> recipeAliases, Dictionary<int, HashSet<string>> inventoryAliases)
        {
            return inventoryAliases.Values.Any(inventoryAliasesForItem =>
                recipeAliases.Overlaps(inventoryAliasesForItem));
        }

        /// <summary>
        /// Performs a fuzzy match operation to identify approximate matches based on predefined criteria.
        /// </summary>
        public void FindFuzzyMatch()
        {
            /* This is a matching scheme that would be used in future improvements on the matching algorithm, 
             * but is not necessary for the current implementation
             */
        }

        #endregion
    }
}
