using Recip_EZ.Server.Data;
using Recip_EZ.Server.Enums;
using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Services
{
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

        public List<int> FindExactMatch(Dictionary<int, HashSet<string>> inventoryAliases, Dictionary<int, HashSet<string>> recipeIngredientAliases)
        {
            List<int> matchedIngredientsIndexList = new List<int>();
            
            foreach(var recipeIngredient in recipeIngredientAliases) 
            {
                int recipeIngredientIndex = recipeIngredient.Key;
                HashSet<string> recipeAliases = recipeIngredient.Value;

                if(FindAliasMatch(recipeIngredientIndex, recipeAliases, inventoryAliases))
                {
                    matchedIngredientsIndexList.Add(recipeIngredientIndex);
                }
            }

            return matchedIngredientsIndexList;
        }

        public bool FindAliasMatch(int recipeIngredientIndex, HashSet<string> recipeAliases, Dictionary<int, HashSet<string>> inventoryAliases)
        {
            return inventoryAliases.Values.Any(inventoryAliasesForItem =>
                recipeAliases.Overlaps(inventoryAliasesForItem));
        }

        public void FindFuzzyMatch()
        {

        }

        public void ResolveBestMatch()
        {

        }
    }
}
