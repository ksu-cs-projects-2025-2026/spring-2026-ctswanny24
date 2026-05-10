using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Recip_EZ.Server.Data;
using Recip_EZ.Server.DTOs;
using Recip_EZ.Server.Enums;
using Recip_EZ.Server.Models;
using System.Text.Json;

namespace Recip_EZ.Server.Services
{
    public class RecipeService
    {
        private sealed class InventoryIngredientCandidate
        {
            public required string DisplayName { get; init; }

            public required HashSet<string> MatchTerms { get; init; }
        }

        private sealed class RecipeIngredientCandidate
        {
            public required string Original { get; init; }

            public required HashSet<string> MatchTerms { get; init; }

            public required string CanonicalTerm { get; init; }
        }

        #region Fields

        private readonly RecipEzDbContext _context;
        private readonly IngredientAliasService _ingredientAliasService;
        private readonly MatchingService _matchingService;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates the RecipeService Layer. Where the DbContext is stored and can be used
        /// </summary>
        /// <param name="context">Db Context</param>
        /// <param name="ingredientAliasService">Shared alias/normalization service.</param>
        public RecipeService(RecipEzDbContext context, IngredientAliasService ingredientAliasService, MatchingService matchingService)
        {
            _context = context;
            _ingredientAliasService = ingredientAliasService;
            _matchingService = matchingService;
        }

        #endregion

        #region CRUD Methods

        /*

        There should NOT be any Create, Update, or Delete methods for recipes. The recipes are meant to be static and unchanging.
        They are only added to the database through the initial seeding process. If there is a need for adding more recipes,
        that should be done through the seeding process and not through the application itself. 
        This is to ensure that the recipes remain consistent and reliable for the users.

        */

        /// <summary>
        /// Method to get ALL recipes in the Database (If for some reason you need to do that)
        /// </summary>
        /// <returns>List of ALL recipe items</returns>
        /// <exception cref="Exception">No recipes found exception</exception>
        public List<Recipe> GetAllRecipes()
        {
            List<Recipe> recipes = _context.Recipes.ToList();

            if (!recipes.Any())
            {
                throw new Exception("No Recipes found in the database.");
            }

            return recipes;
        }

        /// <summary>
        /// Default Get method that will get however many recipes as dictated by n
        /// </summary>
        /// <param name="num">Number of items wanted to be gotten from db</param>
        /// <returns>List of however many recipes dictated by num</returns>
        /// <exception cref="Exception">No recipes found exception</exception>
        public List<Recipe> GetNRecipes(int num)
        {
            List<Recipe> recipes = _context.Recipes.Take(num).ToList();

            if (!recipes.Any())
            {
                throw new Exception("No Recipes found in the database.");
            }

            return recipes;
        }

        /// <summary>
        /// Grabs a recipe by its ID.
        /// </summary>
        /// <param name="id">Id of the wanted recipe</param>
        /// <returns>The recipe with the specified ID</returns>
        /// <exception cref="Exception">Thrown when no recipe is found with the specified ID</exception>
        public Recipe GetRecipeById(int id)
        {
            Recipe? recipe = _context.Recipes.FirstOrDefault(i => i.RecipeId == id);

            if (recipe == null)
            {
                throw new Exception($"No Recipe found with ID: {id}");
            }

            return recipe;
        }

        /// <summary>
        /// Future implementation that will allow for filtering the recipes based on filtering options in the GUI
        /// </summary>
        /// <returns>List of filtered recipes</returns>
        public List<Recipe> FilterRecipes()
        {
            List<Recipe> recipes = _context.Recipes.Take(1).ToList();

            /*Business Logic to be created and used LATER */

            return recipes;
        }

        //To be implemented in V1.0 as part of the scoring algorithm. Will be used to get the top recipes based on their score.
        public List<Recipe> GetRecipesByScore()
        {
            List<Recipe> recipes = _context.Recipes.Take(1).ToList();
            /*Business Logic to be created and used LATER */
            return recipes;
        }

        #endregion

        /// <summary>
        /// Helper method to change Recipe item into RecipeDTO item. Mainly used to convert the stringified lists of ingredients and instructions into actual lists that can be used in the GUI
        /// </summary>
        /// <param name="item">Recipe Item to be changed</param>
        /// <returns>RecipeDTO item for Recipe</returns>
        public RecipeDTO ToDTO(Recipe item)
        {
            var priorities = JsonSerializer.Deserialize<List<string>>(item.Priorities ?? "[]") ?? new List<string>();
            var priorityEnums = new List<Priority>();
            foreach (var priority in priorities)
            {
                priorityEnums.Add((Priority)Enum.Parse(typeof(Priority), priority, true));
            }

            return new RecipeDTO
            {
                RecipeId = item.RecipeId,
                RecipeName = item.RecipeName ?? string.Empty,
                Ingredients = JsonSerializer.Deserialize<List<string>>(item.Ingredients ?? "[]") ?? new List<string>(),
                Instructions = JsonSerializer.Deserialize<List<string>>(item.Instructions ?? "[]") ?? new List<string>(),
                URL = item.URL ?? string.Empty,
                Source = item.Source ?? string.Empty,
                RawIngredientList = JsonSerializer.Deserialize<List<string>>(item.RawIngredientList ?? "[]") ?? new List<string>(),
                CanonIngredients = JsonSerializer.Deserialize<List<string>>(item.CanonIngredients ?? "[]") ?? new List<string>(),
                Priorities = priorityEnums
            };
        }

        #region Heruistic Matching

        //Curation Logic V2
        //Attempted Heuristic Matching. 
        //Core: Proteins, Carbs/Starch, Primary Produce (potatoes, greens, etc.), Structural baking ingredients (yeast, flour, sugar), Major sauce bases (tomato sauce, cream, etc.)
        //Supporting: Secondary produce (onions, peppers, etc.), Spices, Liquids (broth, milk, etc.), cheese/dairy, minor baking ingredients (baking powder, cocoa, etc.), butter/cream
        //Optional: Spices and Garnishes, anything that is "to taste".
        //Each recipe contains many different ingredients in the "raw ingredient" list. This is the basis for this algorithm, and it will checking each list for core ingredients. 
        //Usually, the first few items are the core ingredients, but this is not always the case. So, the algorithm will be checking each ingredient and scoring it based on the presence of core, supporting, and optional ingredients.

        //The three scores for the presence of core, supporting, and optional ingredients will be combined in some way to create an overall score for the recipe.
        private double _coreWeight = 0.6;
        private double _supportingWeight = 0.3;
        private double _optionalWeight = 0.1;

        /// <summary>
        /// Represents an item in the inventory, including its unique identifier and name.
        /// </summary>
        private sealed class InventoryItem
        {
            public int IngredientId { get; init; }
            public string IngredientName { get; init; } = string.Empty;
        }

        /// <summary>
        /// Performs heuristic curation of recipes based on the user's inventory.
        /// </summary>
        /// <param name="userId">The ID of the user for whom to perform curation.</param>
        /// <param name="limit">The maximum number of curated recipes to return.</param>
        /// <param name="minimumMatchPercentage">The minimum match percentage required for a recipe to be included.</param>
        /// <returns>A list of curated recipe DTOs.</returns>
        public List<CuratedRecipeDTO> HeuristicCuration(int userId, int limit = 25, double minimumMatchPercentage = 0)
        {
            //Get the inventory for the user and also get the aliases for each ingredient.
            var inventory = GetUserInventory(userId);

            if(!inventory.Any())
            {
                return new List<CuratedRecipeDTO>();
            }

            //Get the normalized aliases for each ingredient in the inventory.
            Dictionary<int, HashSet<string>> inventoryAliases = GetInventoryAliases(inventory);
            var recipes = GetAllRecipesAsDTO();

            recipes = recipes
                .Where(IsValidForCuration)
                .ToList();



            //Do the same normalization methods for recipe ingredients.
            //Each recipe will have a dictionary where the key is the ingredient index in the raw ingredient list and the value is a hash set of all aliases for that ingredient.
            Dictionary<int, Dictionary<int, HashSet<string>>> recipeAliasCollection = GetRecipeIngredientAliases(recipes);

            Dictionary<int, List<int>> recipeMatchResults = new Dictionary<int, List<int>>();

            foreach(var recipe in recipeAliasCollection)
            {
                List<int> matchedIngredients = _matchingService.FindExactMatch(inventoryAliases, recipe.Value);
                recipeMatchResults.Add(recipe.Key, matchedIngredients);
            }

            List<CuratedRecipeDTO> curatedRecipes = recipes
                .Where(recipe => recipeMatchResults.ContainsKey(recipe.RecipeId))
                .Select(recipe => ToRecipeDTO(recipe, recipeMatchResults[recipe.RecipeId]))
                .ToList();

            //List<CuratedRecipeDTO> curatedRecipes = new List<CuratedRecipeDTO>();

            //foreach(RecipeDTO recipe in recipes)
            //{
            //    curatedRecipes.Add(ToRecipeDTO(recipe, recipeMatchResults[recipe.RecipeId]));
            //}

            return curatedRecipes
                .Where(recipe => recipe.MatchPercentage >= Math.Clamp(minimumMatchPercentage, 0, 100))
                .OrderByDescending(recipe => recipe.CanMakeNow)
                .ThenByDescending(recipe => recipe.Score)
                .ThenByDescending(recipe => recipe.MatchPercentage)
                .Take(limit > 0 ? limit : 25)
                .ToList();
        }

        /// <summary>
        /// Gets the user inventory list for the specified userId.
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns>List of InventoryItem objects representing the user's inventory</returns>
        private List<InventoryItem> GetUserInventory(int userId)
        {
            return _context.Ingredients.AsNoTracking().Join(_context.UserInventories.Where(ui => ui.UserId == userId),
                ingredient => ingredient.IngredientId,
                ui => ui.IngredientId,
                (ingredient, ui) => new InventoryItem
                {
                    IngredientId = ingredient.IngredientId,
                    IngredientName = ingredient.Name ?? string.Empty
                })
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Grabs every recipe and converts it to a DTO
        /// </summary>
        /// <returns>List of RecipeDTO objects representing the recipes</returns>
        private List<RecipeDTO> GetAllRecipesAsDTO()
        {
            return _context.Recipes
                .AsNoTracking()
                .ToList()
                .Select(recipe => ToDTO(recipe))
                .ToList();
        }

        /// <summary>
        /// Creates a dictionary for all ingredient aliases of the items in the inventory.
        /// </summary>
        /// <param name="inventory">Current Inventory of the User</param>
        /// <returns>Dictionary where the key is the ingredientID and the value is a hash set of all aliases for that ingredient</returns>
        private Dictionary<int, HashSet<String>> GetInventoryAliases(List<InventoryItem> inventory)
        {
            //Create a dictionary where the key is the ingredientId and the value is a hash set of all aliases for that ingredient.
            //This will be used to quickly look up the aliases for each ingredient in the inventory when scoring the recipes.
            var inventoryAliases = new Dictionary<int, HashSet<string>>();

            //For each ingredient in the inventory, get the aliases and add them to the dictionary.
            foreach (var ingredient in inventory)
            {
                inventoryAliases.Add(ingredient.IngredientId, _ingredientAliasService.GetAliasesForIngredientName(ingredient.IngredientName));
            }

            return inventoryAliases;
        }

        /// <summary>
        /// Builds a mapping of recipe and ingredient identifiers to their associated ingredient alias sets for a
        /// collection of recipes.
        /// </summary>
        /// <remarks>Each inner dictionary maps the zero-based index of an ingredient in the recipe's
        /// canonical ingredient list to a set of alias names for that ingredient. Recipes without canonical ingredients
        /// are skipped.</remarks>
        /// <param name="recipes">A list of recipe data transfer objects for which to retrieve ingredient alias mappings. Cannot be null.</param>
        /// <returns>A dictionary where each key is a recipe identifier and each value is a dictionary mapping ingredient indices
        /// to sets of alias strings for that ingredient. The returned dictionary is empty if no valid recipes are
        /// provided.</returns>
        private Dictionary<int, Dictionary<int, HashSet<string>>> GetRecipeIngredientAliases(List<RecipeDTO> recipes)
        {
            //THIS IS A DICTIONARY FOR EACH RECIPE.
            //THE KEY IS THE RECIPE ID AND THE VALUE IS THE DICTIONARY FOR ALL THE INGREDIENTS AND THEIR CORRESPONDING ALIAS HASH SETS
            Dictionary<int, Dictionary<int, HashSet<string>>> recipeAliasLists = new Dictionary<int, Dictionary<int, HashSet<string>>>();

            //Go through each recipe.
            foreach (var recipe in recipes)
            {
                //If there's no item in the recipe canonIngredients(even though there shouldn't be) continue to next recipe.
                if (recipe.CanonIngredients == null || recipe.CanonIngredients.Count == 0)
                {
                    continue;
                }

                //Add a new hashset for ingredient aliases for each ingredient in each recipe.
                //The key is the index of the ingredient in the canonIngredients list and the value is the hashset of aliases for that ingredient.
                var recipeIngredientAliases = new Dictionary<int, HashSet<string>>();
                for(int i = 0; i < recipe.CanonIngredients.Count; i++)
                {
                    recipeIngredientAliases.Add(i, _ingredientAliasService.GetAliasesForIngredientName(recipe.CanonIngredients[i]));
                }

                //Add the recipe dictionary to the main dictionary with the recipe ID as the key and the ingredient alias dictionary as the value.
                recipeAliasLists.Add(recipe.RecipeId, recipeIngredientAliases);
            }

            return recipeAliasLists;
        }

        /// <summary>
        /// Converts a Recipe object and a list of matched ingredient indexes to a CuratedRecipeDTO instance.
        /// </summary>
        /// <param name="recipe">The Recipe object to convert. Cannot be null.</param>
        /// <param name="matchedIndexes">A list of zero-based indexes indicating which ingredients in the recipe matched the search criteria. Cannot
        /// be null.</param>
        /// <returns>A CuratedRecipeDTO representing the provided recipe and the specified matched ingredient indexes.</returns>
        private CuratedRecipeDTO ToRecipeDTO(RecipeDTO recipe, List<int> matchedIndexes)
        {
            List<string> matchedCoreIngredients = new List<string>();
            List<string> missingCoreIngredients = new List<string>();

            List<string> matchedSupportingIngredients = new List<string>();
            List<string> missingSupportingIngredients = new List<string>();

            List<string> matchedOptionalIngredients = new List<string>();
            List<string> missingOptionalIngredients = new List<string>();



            for (int i = 0; i < recipe.CanonIngredients?.Count; i++)
            {
                switch(recipe.Priorities![i])
                {
                    case Priority.Core:
                        if (matchedIndexes.Contains(i))
                        {
                            matchedCoreIngredients.Add(recipe.CanonIngredients[i]);
                        }
                        else
                        {
                            missingCoreIngredients.Add(recipe.CanonIngredients[i]);
                        }
                        break;
                    case Priority.Supporting:
                        if (matchedIndexes.Contains(i))
                        {
                            matchedSupportingIngredients.Add(recipe.CanonIngredients[i]);
                        }
                        else
                        {
                            missingSupportingIngredients.Add(recipe.CanonIngredients[i]);
                        }
                        break;
                    case Priority.Optional:
                        if (matchedIndexes.Contains(i))
                        {
                            matchedOptionalIngredients.Add(recipe.CanonIngredients[i]);
                        }
                        else
                        {
                            missingOptionalIngredients.Add(recipe.CanonIngredients[i]);
                        }
                        break;
                }


            }

            List<int> counts = new List<int>();
            counts.Add(matchedCoreIngredients.Count);
            counts.Add(missingCoreIngredients.Count);

            counts.Add(matchedSupportingIngredients.Count);
            counts.Add(missingSupportingIngredients.Count);

            counts.Add(matchedOptionalIngredients.Count);
            counts.Add(missingOptionalIngredients.Count);

            List<double> scores = ScoreRecipe(counts);

            CuratedRecipeDTO curatedRecipeDTO = new CuratedRecipeDTO()
            {
                RecipeId = recipe.RecipeId,
                RecipeName = recipe.RecipeName!,
                Ingredients = recipe.Ingredients!,
                Instructions = recipe.Instructions!,
                URL = recipe.URL!,
                Source = recipe.Source!,
                RawIngredientList = recipe.RawIngredientList!,
                MatchedIngredients = matchedIndexes.Select(index => recipe.CanonIngredients![index]).ToList(),
                MissingIngredients = recipe.CanonIngredients!.Where((ingredient, index) => !matchedIndexes.Contains(index)).ToList(),
                MatchedIngredientCount = matchedIndexes.Count,
                MissingIngredientCount = (recipe.CanonIngredients?.Count ?? 0) - matchedIndexes.Count,
                TotalIngredientCount = recipe.CanonIngredients?.Count ?? 0,
                MatchPercentage = Math.Round(((double)matchedIndexes.Count / (recipe.CanonIngredients?.Count ?? 1)) * 100, 2), // Avoid division by zero
                CanMakeNow = (scores[0] >= 0.75),
                IsCloseMatch = ((double)matchedIndexes.Count / (recipe.CanonIngredients?.Count ?? 1)) >= 0.5, // Arbitrary threshold for "close match"
                MatchedCoreIngredients = matchedCoreIngredients,
                MissingCoreIngredients = missingCoreIngredients,
                MatchedSupportingIngredients = matchedSupportingIngredients,
                MissingSupportingIngredients = missingSupportingIngredients,
                MatchedOptionalIngredients = matchedOptionalIngredients,
                MissingOptionalIngredients = missingOptionalIngredients,
                Score = scores[0],
                CoreScore = scores[1],
                SupportingScore = scores[2],
                OptionalScore = scores[3],
            };

            return curatedRecipeDTO;

        }

        /// <summary>
        /// This calcualtes the scores for the recipe. 
        /// Core Score: (matched core ingredient count) / (matched core ingredient count + missing core ingredient count)
        /// Supporting Score: (matched supporting ingredient count) / (matched supporting ingredient count + missing supporting ingredient count)
        /// Optional Score: (matched optional ingredient count) / (matched optional ingredient count + missing optional ingredient count)
        /// Final Heuristic Score: (core score * core weight) + (supporting score * supporting weight) + (optional score * optional weight)
        /// </summary>
        /// <param name="matchedCoreCount">The number of matched core ingredients.</param>
        /// <param name="matchedSupportingIngredients">The number of matched supporting ingredients.</param>
        /// <param name="matchedOptionalIngredients">The number of matched optional ingredients.</param>
        /// <param name="missingCoreCount">The number of missing core ingredients.</param>
        /// <param name="missingSupportingCount">The number of missing supporting ingredients.</param>
        /// <param name="missingOptionalCount">The number of missing optional ingredients.</param>
        /// <returns>A list of scores: [heuristicScore, coreScore, supportingScore, optionalScore]</returns>
        private List<double> ScoreRecipe(List<int> counts)
        {
            List<double> scores = new List<double>();

            double coreScore = CalculateCategoryScore(counts[0], counts[1]);
            double supportingScore = CalculateCategoryScore(counts[2], counts[3]);
            double optionalScore = CalculateCategoryScore(counts[4], counts[5]);

            double activeWeightTotal = 0;
            activeWeightTotal += HasIngredients(counts[0], counts[1]) ? _coreWeight : 0;
            activeWeightTotal += HasIngredients(counts[2], counts[3]) ? _supportingWeight : 0;
            activeWeightTotal += HasIngredients(counts[4], counts[5]) ? _optionalWeight : 0;

            //Calculate the final heuristic score using the weights for each category. The weights can be adjusted to give more or less importance to each category.
            double heuristicScore = activeWeightTotal == 0
                ? 0
                : ((coreScore * _coreWeight) + (supportingScore * _supportingWeight) + (optionalScore * _optionalWeight)) / activeWeightTotal;

            scores.Add(Math.Round(heuristicScore, 2));
            scores.Add(Math.Round(coreScore, 2));
            scores.Add(Math.Round(supportingScore, 2));
            scores.Add(Math.Round(optionalScore, 2));

            return scores;
        }

        /// <summary>
        /// Calculates the proportion of matched items within a category based on the number of matched and missing
        /// items.
        /// </summary>
        /// <param name="matchedCount">The number of items that were successfully matched. Must be zero or greater.</param>
        /// <param name="missingCount">The number of items that were missing or not matched. Must be zero or greater.</param>
        /// <returns>A double value representing the ratio of matched items to the total number of items. Returns 0 if both
        /// matched and missing counts are zero.</returns>
        private double CalculateCategoryScore(int matchedCount, int missingCount)
        {
            int totalCount = matchedCount + missingCount;
            return totalCount == 0 ? 0 : (double)matchedCount / totalCount;
        }

        /// <summary>
        /// Determines whether there are any ingredients present based on the specified matched and missing counts.
        /// </summary>
        /// <param name="matchedCount">The number of ingredients that are matched or available.</param>
        /// <param name="missingCount">The number of ingredients that are missing or unavailable.</param>
        /// <returns>true if the total number of matched and missing ingredients is greater than zero; otherwise, false.</returns>
        private bool HasIngredients(int matchedCount, int missingCount)
        {
            return matchedCount + missingCount > 0;
        }

        /// <summary>
        /// Determines whether the specified recipe contains valid ingredient and priority data for curation.
        /// </summary>
        /// <param name="recipe">The recipe to validate for curation. Must not be null and must contain non-empty ingredient and priority
        /// lists of equal length.</param>
        /// <returns>true if the recipe has non-null, non-empty ingredient and priority lists of equal length; otherwise, false.</returns>
        private bool IsValidForCuration(RecipeDTO recipe)
        {
            return recipe.CanonIngredients != null
                && recipe.CanonIngredients.Count > 0
                && recipe.Priorities != null
                && recipe.Priorities.Count == recipe.CanonIngredients.Count;
        }


        #endregion
    }
}
