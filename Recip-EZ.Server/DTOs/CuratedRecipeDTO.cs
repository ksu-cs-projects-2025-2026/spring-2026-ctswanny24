namespace Recip_EZ.Server.DTOs
{
    public class CuratedRecipeDTO
    {
        public int RecipeId { get; init; }

        public string RecipeName { get; init; } = string.Empty;

        public List<string> Ingredients { get; init; } = new();

        public List<string> Instructions { get; init; } = new();

        public string URL { get; init; } = string.Empty;

        public string Source { get; init; } = string.Empty;

        public List<string> RawIngredientList { get; init; } = new();

        public List<string> MatchedIngredients { get; init; } = new();

        public List<string> MissingIngredients { get; init; } = new();

        public int MatchedIngredientCount { get; init; }

        public int MissingIngredientCount { get; init; }

        public int TotalIngredientCount { get; init; }

        public double MatchPercentage { get; init; }

        public bool CanMakeNow { get; init; }

        public bool IsCloseMatch { get; init; }

        public double Score { get; init; }

        public double CoreScore { get; init; }

        public double SupportingScore { get; init; }

        public double OptionalScore { get; init; }

        public List<string> MatchedCoreIngredients { get; init; } = new();

        public List<string> MissingCoreIngredients { get; init; } = new();

        public List<string> MatchedSupportingIngredients { get; init; } = new();

        public List<string> MissingSupportingIngredients { get; init; } = new();

        public List<string> MatchedOptionalIngredients { get; init; } = new();

        public List<string> MissingOptionalIngredients { get; init; } = new();
    }
}
