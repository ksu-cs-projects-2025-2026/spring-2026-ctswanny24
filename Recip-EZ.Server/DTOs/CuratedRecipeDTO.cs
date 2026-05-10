namespace Recip_EZ.Server.DTOs
{
    public class CuratedRecipeDTO
    {
        /// <summary>
        /// Gets the unique identifier for the recipe.
        /// </summary>
        public int RecipeId { get; init; }

        /// <summary>
        /// Gets the name of the recipe.
        /// </summary>
        public string RecipeName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the list of ingredients required for the recipe.
        /// </summary>
        public List<string> Ingredients { get; init; } = new();

        /// <summary>
        /// Gets the collection of instructions associated with this instance.
        /// </summary>
        public List<string> Instructions { get; init; } = new();

        /// <summary>
        /// Gets the URL associated with this instance.
        /// </summary>
        public string URL { get; init; } = string.Empty;

        /// <summary>
        /// Gets the source identifier associated with this instance.
        /// </summary>
        public string Source { get; init; } = string.Empty;

        /// <summary>
        /// List of raw ingredients used in the recipe
        /// </summary>
        public List<string> RawIngredientList { get; init; } = new();

        /// <summary>
        /// List of canonical ingredients used in the recipe that are matched with the current user's inventory.
        /// </summary>
        public List<string> MatchedIngredients { get; init; } = new();

        /// <summary>
        /// List of canonical ingredients that are missing from the user's inventory but are required for the recipe
        /// </summary>
        public List<string> MissingIngredients { get; init; } = new();

        /// <summary>
        /// Count of the matched ingredients
        /// </summary>
        public int MatchedIngredientCount { get; init; }

        /// <summary>
        /// Count of the missing ingredients that are required for the recipe
        /// </summary>
        public int MissingIngredientCount { get; init; }

        /// <summary>
        /// Total ingredient count for the recipe
        /// </summary>
        public int TotalIngredientCount { get; init; }

        /// <summary>
        /// Basic match percentage calculated as (MatchedIngredientCount / TotalIngredientCount) * 100
        /// </summary>
        public double MatchPercentage { get; init; }

        /// <summary>
        /// Boolean to determine if the recipe could ideally be made at this time
        /// </summary>
        public bool CanMakeNow { get; init; }

        /// <summary>
        /// If the user is past a certain score in matching, it is a close match and they should be encouraged to make the recipe.
        /// </summary>
        public bool IsCloseMatch { get; init; }

        /// <summary>
        /// Heuristic score calculated based on the priorities of the matched and missing ingredients
        /// </summary>
        public double Score { get; init; }

        /// <summary>
        /// Score of core ingredients matched, calculated as (MatchedCoreIngredients.Count / TotalCoreIngredients) * 100
        /// </summary>
        public double CoreScore { get; init; }

        /// <summary>
        /// Score of supporting ingredients matched, calculated as (MatchedSupportingIngredients.Count / TotalSupportingIngredients) * 100
        /// </summary>
        public double SupportingScore { get; init; }

        /// <summary>
        /// Score of optional ingredients matched, calculated as (MatchedOptionalIngredients.Count / TotalOptionalIngredients) * 100
        /// </summary>
        public double OptionalScore { get; init; }

        /// <summary>
        /// List of matched core ingredients that are required for the recipe and are present in the user's inventory
        /// </summary>
        public List<string> MatchedCoreIngredients { get; init; } = new();

        /// <summary>
        /// List of missing core ingredients that are required for the recipe
        /// </summary>
        public List<string> MissingCoreIngredients { get; init; } = new();

        /// <summary>
        /// List of matched supporting ingredients that are required for the recipe and are present in the user's inventory
        /// </summary>
        public List<string> MatchedSupportingIngredients { get; init; } = new();

        /// <summary>
        /// List of missing supporting ingredients that are required for the recipe
        /// </summary>
        public List<string> MissingSupportingIngredients { get; init; } = new();

        /// <summary>
        /// List of matched optional ingredients that are required for the recipe and are present in the user's inventory
        /// </summary>
        public List<string> MatchedOptionalIngredients { get; init; } = new();

        /// <summary>
        /// List of missing optional ingredients that are required for the recipe
        /// </summary>
        public List<string> MissingOptionalIngredients { get; init; } = new();
    }
}
