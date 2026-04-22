using Microsoft.EntityFrameworkCore;
using Recip_EZ.Server.Data;
using Recip_EZ.Server.DTOs;
using Recip_EZ.Server.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Recip_EZ.Server.Services
{
    public class RecipeService
    {
        private sealed class InventoryIngredientCandidate
        {
            public required string DisplayName { get; init; }

            public required HashSet<string> MatchTerms { get; init; }
        }

        #region Fields

        private readonly RecipEzDbContext _context;

        private static readonly HashSet<string> IgnoredIngredientWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "a",
            "an",
            "and",
            "boneless",
            "cooked",
            "crushed",
            "diced",
            "dried",
            "extra",
            "fresh",
            "fried",
            "grated",
            "ground",
            "large",
            "medium",
            "minced",
            "of",
            "optional",
            "raw",
            "roasted",
            "shredded",
            "skinless",
            "sliced",
            "small",
            "taste",
            "to",
            "virgin"
        };

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates the RecipeService Layer. Where the DbContext is stored and can be used
        /// </summary>
        /// <param name="context">Db Context</param>
        public RecipeService(RecipEzDbContext context)
        {
            _context = context;
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

            if (inventoryIngredients.Count == 0)
            {
                return new List<CuratedRecipeDTO>();
            }

            var inventoryIngredientIds = inventoryIngredients
                .Select(item => item.IngredientId)
                .ToList();

            var aliasesByIngredientId = _context.Aliases
                .Where(alias => inventoryIngredientIds.Contains(alias.IngredientId))
                .AsNoTracking()
                .ToList()
                .GroupBy(alias => alias.IngredientId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(alias => alias.AliasName).ToList());

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
                RawIngredientList = JsonSerializer.Deserialize<List<string>>(item.RawIngredientList ?? "[]") ?? new List<string>()
            };
        }

        private CuratedRecipeDTO BuildCuratedRecipe(Recipe recipe, List<InventoryIngredientCandidate> inventoryCandidates)
        {
            var recipeDto = ToDTO(recipe);
            var distinctRecipeIngredients = recipeDto.RawIngredientList
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => new
                {
                    Original = item.Trim(),
                    Normalized = NormalizeIngredient(item)
                })
                .Where(item => !string.IsNullOrWhiteSpace(item.Normalized))
                .GroupBy(item => item.Normalized)
                .Select(group => group.First())
                .ToList();

            List<string> matchedIngredients = new();
            List<string> missingIngredients = new();

            foreach (var recipeIngredient in distinctRecipeIngredients)
            {
                var matchingInventoryIngredient = FindBestInventoryMatch(recipeIngredient.Normalized, inventoryCandidates);

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

        private InventoryIngredientCandidate BuildInventoryCandidate(string ingredientName, IEnumerable<string> aliases)
        {
            var matchTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            AddMatchTerm(matchTerms, ingredientName);

            foreach (var alias in aliases)
            {
                AddMatchTerm(matchTerms, alias);
            }

            return new InventoryIngredientCandidate
            {
                DisplayName = ingredientName,
                MatchTerms = matchTerms
            };
        }

        private InventoryIngredientCandidate? FindBestInventoryMatch(
            string normalizedRecipeIngredient,
            IEnumerable<InventoryIngredientCandidate> inventoryCandidates)
        {
            var exactMatch = inventoryCandidates
                .FirstOrDefault(candidate => candidate.MatchTerms.Contains(normalizedRecipeIngredient));

            if (exactMatch != null)
            {
                return exactMatch;
            }

            return inventoryCandidates
                .Select(candidate => new
                {
                    Candidate = candidate,
                    Score = candidate.MatchTerms
                        .Select(term => ScorePhraseMatch(normalizedRecipeIngredient, term))
                        .Max()
                })
                .Where(item => item.Score > 0)
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Candidate.DisplayName.Length)
                .Select(item => item.Candidate)
                .FirstOrDefault();
        }

        private void AddMatchTerm(HashSet<string> matchTerms, string? value)
        {
            var normalized = NormalizeIngredient(value);

            if (!string.IsNullOrWhiteSpace(normalized))
            {
                matchTerms.Add(normalized);
            }
        }

        private string NormalizeIngredient(string? ingredient)
        {
            if (string.IsNullOrWhiteSpace(ingredient))
            {
                return string.Empty;
            }

            var cleanedIngredient = ingredient.Trim().ToLowerInvariant();
            cleanedIngredient = cleanedIngredient.Replace("&", " and ");
            cleanedIngredient = Regex.Replace(cleanedIngredient, @"[\d]", " ");
            cleanedIngredient = Regex.Replace(cleanedIngredient, @"[^\w\s]", " ");

            var normalizedWords = cleanedIngredient
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(word => !IgnoredIngredientWords.Contains(word))
                .Select(SingularizeWord)
                .Where(word => !string.IsNullOrWhiteSpace(word))
                .ToList();

            return string.Join(' ', normalizedWords);
        }

        private string SingularizeWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word) || word.Length <= 2)
            {
                return word;
            }

            if (word.EndsWith("ies", StringComparison.OrdinalIgnoreCase) && word.Length > 3)
            {
                return word[..^3] + "y";
            }

            if (word.EndsWith("es", StringComparison.OrdinalIgnoreCase) &&
                word.Length > 3 &&
                !word.EndsWith("ses", StringComparison.OrdinalIgnoreCase))
            {
                return word[..^2];
            }

            if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) &&
                word.Length > 3 &&
                !word.EndsWith("ss", StringComparison.OrdinalIgnoreCase))
            {
                return word[..^1];
            }

            return word;
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
    }
}
