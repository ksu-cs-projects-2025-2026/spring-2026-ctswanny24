using Microsoft.EntityFrameworkCore;
using Recip_EZ.Server.Data;
using Recip_EZ.Server.DTOs;
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

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates the RecipeService Layer. Where the DbContext is stored and can be used
        /// </summary>
        /// <param name="context">Db Context</param>
        /// <param name="ingredientAliasService">Shared alias/normalization service.</param>
        public RecipeService(RecipEzDbContext context, IngredientAliasService ingredientAliasService)
        {
            _context = context;
            _ingredientAliasService = ingredientAliasService;
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

        /// <summary>
        /// Gets curated recipes for the specified user based on ingredient overlap with the user's inventory.
        /// </summary>
        /// <param name="userId">The user whose inventory should be compared to the recipe ingredient lists.</param>
        /// <param name="limit">Maximum number of curated recipes to return.</param>
        /// <param name="minimumMatchPercentage">Optional threshold from 0 to 100 for filtering out weak matches.</param>
        /// <returns>Sorted list of curated recipes for the user.</returns>
        public List<CuratedRecipeDTO> GetCuratedRecipesForUser(int userId, int limit = 25, double minimumMatchPercentage = 0)
        {
            limit = limit <= 0 ? 25 : limit;
            minimumMatchPercentage = Math.Clamp(minimumMatchPercentage, 0, 100);

            //Grabs all the distinct inventory ingredients corresponding to the specific userId.
            var inventoryIngredients = _context.UserInventories
                .Where(ui => ui.UserId == userId)
                .Join(_context.Ingredients,
                    ui => ui.IngredientId,
                    ingredient => ingredient.IngredientId,
                    (ui, ingredient) => new
                    {
                        ingredient.IngredientId,
                        IngredientName = ingredient.Name ?? string.Empty
                    })
                .Distinct()
                .ToList();

            //If that userInventory is empty, return empty list.
            if (inventoryIngredients.Count == 0)
            {
                return new List<CuratedRecipeDTO>();
            }

            //Now, get all of the ids of the inventory ingredients.
            var inventoryIngredientIds = inventoryIngredients
                .Select(item => item.IngredientId)
                .ToList();

            //Now grab all the aliases for the ingredients in the inventory
            var aliasesByIngredientId = _context.IngredientAliases
                .Where(alias => inventoryIngredientIds.Contains(alias.IngredientId))
                .AsNoTracking()
                .ToList()
                .GroupBy(alias => alias.IngredientId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(alias => alias.AliasName).ToList());

            //
            var inventoryCandidates = inventoryIngredients
                .Select(item => BuildInventoryCandidate(
                    item.IngredientName,
                    aliasesByIngredientId.GetValueOrDefault(item.IngredientId, new List<string>())))
                .ToList();

            return _context.Recipes
                .AsNoTracking()
                .ToList()
                .Select(recipe => BuildCuratedRecipe(recipe, inventoryCandidates))
                .Where(recipe => recipe.TotalIngredientCount > 0)
                .Where(recipe => recipe.MatchedIngredientCount > 0)
                .Where(recipe => recipe.MatchPercentage >= minimumMatchPercentage)
                .OrderByDescending(recipe => recipe.CanMakeNow)
                .ThenByDescending(recipe => recipe.MatchPercentage)
                .ThenBy(recipe => recipe.MissingIngredientCount)
                .ThenBy(recipe => recipe.RecipeName)
                .Take(limit)
                .ToList();
        }

        #endregion

        #region Helper Methods

        //Future method for scoring algorithm. V1.0 addition.
        public int ScoreRecipe()
        {
            /*Business Logic to be created and used LATER */
            return 0;
        }

        /// <summary>
        /// Helper method to change Recipe item into RecipeDTO item. Mainly used to convert the stringified lists of ingredients and instructions into actual lists that can be used in the GUI
        /// </summary>
        /// <param name="item">Recipe Item to be changed</param>
        /// <returns>RecipeDTO item for Recipe</returns>
        public RecipeDTO ToDTO(Recipe item)
        {
            return new RecipeDTO
            {
                RecipeId = item.RecipeId,
                RecipeName = item.RecipeName ?? string.Empty,
                Ingredients = JsonSerializer.Deserialize<List<string>>(item.Ingredients ?? "[]") ?? new List<string>(),
                Instructions = JsonSerializer.Deserialize<List<string>>(item.Instructions ?? "[]") ?? new List<string>(),
                URL = item.URL ?? string.Empty,
                Source = item.Source ?? string.Empty,
                RawIngredientList = JsonSerializer.Deserialize<List<string>>(item.RawIngredientList ?? "[]") ?? new List<string>(),
                CoreIngredients = new List<Ingredient>(),
                SupportIngredients = new List<Ingredient>(),
                OptionalIngredients = new List<Ingredient>()
            };
        }

        private CuratedRecipeDTO BuildCuratedRecipe(Recipe recipe, List<InventoryIngredientCandidate> inventoryCandidates)
        {
            var recipeDto = ToDTO(recipe);
            var distinctRecipeIngredients = recipeDto.RawIngredientList
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => BuildRecipeIngredientCandidate(item))
                .Where(item => !string.IsNullOrWhiteSpace(item.CanonicalTerm))
                .GroupBy(item => item.CanonicalTerm)
                .Select(group => group.First())
                .ToList();

            List<string> matchedIngredients = new();
            List<string> missingIngredients = new();

            foreach (var recipeIngredient in distinctRecipeIngredients)
            {
                var matchingInventoryIngredient = FindBestInventoryMatch(recipeIngredient.MatchTerms, inventoryCandidates);

                if (matchingInventoryIngredient == null)
                {
                    missingIngredients.Add(recipeIngredient.Original);
                    continue;
                }

                matchedIngredients.Add(recipeIngredient.Original);
            }

            var totalIngredientCount = distinctRecipeIngredients.Count;
            var matchedIngredientCount = matchedIngredients.Count;
            var missingIngredientCount = missingIngredients.Count;
            var matchPercentage = totalIngredientCount == 0
                ? 0
                : Math.Round((double)matchedIngredientCount / totalIngredientCount * 100, 1);

            return new CuratedRecipeDTO
            {
                RecipeId = recipeDto.RecipeId,
                RecipeName = recipeDto.RecipeName,
                Ingredients = recipeDto.Ingredients,
                Instructions = recipeDto.Instructions,
                URL = recipeDto.URL,
                Source = recipeDto.Source,
                RawIngredientList = recipeDto.RawIngredientList,
                MatchedIngredients = matchedIngredients,
                MissingIngredients = missingIngredients,
                MatchedIngredientCount = matchedIngredientCount,
                MissingIngredientCount = missingIngredientCount,
                TotalIngredientCount = totalIngredientCount,
                MatchPercentage = matchPercentage,
                CanMakeNow = totalIngredientCount > 0 && missingIngredientCount == 0,
                IsCloseMatch = missingIngredientCount > 0 &&
                               (missingIngredientCount <= 2 || matchPercentage >= 60)
            };
        }

        //Helps create a new item that contains all aliases in a hash set for the ingredient that is passed in.
        private RecipeIngredientCandidate BuildRecipeIngredientCandidate(string rawIngredient)
        {
            var matchTerms = _ingredientAliasService.GetAliasesForIngredientName(rawIngredient);
            var canonicalTerm = _ingredientAliasService.NormalizeIngredient(rawIngredient);

            return new RecipeIngredientCandidate
            {
                Original = rawIngredient.Trim(),
                MatchTerms = matchTerms,
                CanonicalTerm = canonicalTerm
            };
        }

        //
        private InventoryIngredientCandidate BuildInventoryCandidate(string ingredientName, IEnumerable<string> aliases)
        {
            var matchTerms = _ingredientAliasService.GetAliasesForIngredientName(ingredientName);

            foreach (var alias in aliases)
            {
                foreach (var generatedAlias in _ingredientAliasService.GetAliasesForIngredientName(alias))
                {
                    matchTerms.Add(generatedAlias);
                }
            }

            return new InventoryIngredientCandidate
            {
                DisplayName = ingredientName,
                MatchTerms = matchTerms
            };
        }

        private InventoryIngredientCandidate? FindBestInventoryMatch(
            HashSet<string> recipeIngredientTerms,
            IEnumerable<InventoryIngredientCandidate> inventoryCandidates)
        {
            var exactMatch = inventoryCandidates
                .FirstOrDefault(candidate => candidate.MatchTerms.Overlaps(recipeIngredientTerms));

            if (exactMatch != null)
            {
                return exactMatch;
            }

            return inventoryCandidates
                .Select(candidate => new
                {
                    Candidate = candidate,
                    Score = recipeIngredientTerms
                        .SelectMany(recipeTerm => candidate.MatchTerms,
                            (recipeTerm, candidateTerm) => ScorePhraseMatch(recipeTerm, candidateTerm))
                        .DefaultIfEmpty(0)
                        .Max()
                })
                .Where(item => item.Score > 0)
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Candidate.DisplayName.Length)
                .Select(item => item.Candidate)
                .FirstOrDefault();
        }

        private int ScorePhraseMatch(string recipeIngredient, string candidateTerm)
        {
            if (string.IsNullOrWhiteSpace(recipeIngredient) || string.IsNullOrWhiteSpace(candidateTerm))
            {
                return 0;
            }

            if (recipeIngredient.Equals(candidateTerm, StringComparison.OrdinalIgnoreCase))
            {
                return 100;
            }

            var recipeWords = recipeIngredient
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var candidateWords = candidateTerm
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var sharedWords = recipeWords.Intersect(candidateWords, StringComparer.OrdinalIgnoreCase).Count();

            if (sharedWords >= 2 && sharedWords == Math.Min(recipeWords.Count, candidateWords.Count))
            {
                return sharedWords * 10;
            }

            return 0;
        }

        #endregion

        //Curation Logic V2
        //Attempted Heuristic Matching. 
        //Core: Proteins, Carbs/Starch, Primary Produce (potatoes, greens, etc.), Structural baking ingredients (yeast, flour, sugar), Major sauce bases (tomato sauce, cream, etc.)
        //Supporting: Secondary produce (onions, peppers, etc.), Spices, Liquids (broth, milk, etc.), cheese/dairy, minor baking ingredients (baking powder, cocoa, etc.), butter/cream
        //Optional: Spices and Garnishes, anything that is "to taste".
        //Each recipe contains many different ingredients in the "raw ingredient" list. This is the basis for this algorithm, and it will checking each list for core ingredients. 
        //Usually, the first few items are the core ingredients, but this is not always the case. So, the algorithm will be checking each ingredient and scoring it based on the presence of core, supporting, and optional ingredients.
        
        //The three scores for the presence of core, supporting, and optional ingredients will be combined in some way to create an overall score for the recipe.
        private double _coreScore = 0;
        private double _coreWeight = 0.6;

        private double _supportingScore = 0;
        private double _supportingWeight = 0.3;

        private double _optionalScore = 0;
        private double _optionalWeight = 0.1;


        public List<CuratedRecipeDTO> ComplicatedCuration(int userId, int limit = 25, double minimumMatchPercentage = 0)
        {
            //Step 1: Get all recipes from the database. 
            var recipes = _context.Recipes
                .AsNoTracking()
                .ToList()
                .Select(recipe => ToDTO(recipe))
                .ToList();

            var ingredients = _context.Ingredients
                .Join(_context.IngredientAliases, i => i.IngredientId, a => a.IngredientId, (i, a) => new { i, a })
                .AsNoTracking()
                .ToList();


            foreach (var recipe in recipes)
            {
                for (int i = 0; i < recipe.RawIngredientList.Count; i++)
                {
                    //Going through every ingredient in the raw ingredient list and building a candidate that contains all of the match terms for that ingredient.
                    var recipeIngredient = BuildRecipeIngredientCandidate(recipe.RawIngredientList[i]);

                    var j = FindBestInventoryMatch(recipeIngredient.MatchTerms, new List<InventoryIngredientCandidate>());

                    //Use the MatchTerms of the recipe ingredient to find the best matching inventory ingredient candidate. 
                    //This will be used to determine if the ingredient is present in the inventory and to calculate the scores for core, supporting, and optional ingredients based on the presence of matching inventory ingredients.
                }
            }
            /*Business Logic to be created and used LATER */
            return new List<CuratedRecipeDTO>();
        }
    }
}
