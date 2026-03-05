using Recip_EZ.Server.Interfaces;
using Recip_EZ.Server.Enums;

namespace Recip_EZ.Server.Models
{
    /// <summary>
    /// Model representing a recipe.
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// Id of the recipe in the database
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Name of the recipe
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Instructions for the recipe
        /// </summary>
        public string Instructions { get; init; }

        /// <summary>
        /// URL to access the recipe online.
        /// </summary>
        public string URL { get; init; }

        /// <summary>
        /// Source from where the recipe was obtained.
        /// </summary>
        public string Source { get; init; }
    
        /// <summary>
        /// Description of the recipe.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Difficulty of the recipe (to help with future filtering and sorting)
        /// </summary>
        public Difficulty Difficulty { get; init; }

        /// <summary>
        /// Time it takes to make the recipe in minutes.
        /// </summary>
        public int PrepTime { get; init; }

        
        public Recipe()
        {

        }
    }
}
