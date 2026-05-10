using Recip_EZ.Server.Models;
using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.DTOs
{
    public class RecipeDTO
    {
        /// <summary>
        /// Id of the recipe in the database
        /// </summary>
        public int RecipeId { get; init; }

        /// <summary>
        /// Name of the recipe
        /// </summary>
        public string? RecipeName { get; init; }

        public List<string>? Ingredients { get; init; }

        /// <summary>
        /// Instructions for the recipe
        /// </summary>
        public List<string>? Instructions { get; init; }

        /// <summary>
        /// URL to access the recipe online.
        /// </summary>
        public string? URL { get; init; }

        /// <summary>
        /// Source from where the recipe was obtained.
        /// </summary>
        public string? Source { get; init; }

        /// <summary>
        /// Description of the recipe.
        /// </summary>
        public List<string>? RawIngredientList { get; init; }

        public List<string>? CanonIngredients { get; init; }

        public List<Priority>? Priorities { get; init; }

        public List<Ingredient>? CoreIngredients { get; set; }

        public List<Ingredient>? SupportIngredients { get; set; }

        public List<Ingredient>? OptionalIngredients { get; set; }

        }
    }
