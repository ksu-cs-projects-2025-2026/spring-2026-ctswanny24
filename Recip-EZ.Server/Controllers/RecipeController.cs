using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recip_EZ.Server.DTOs;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using System.Security.Claims;

namespace Recip_EZ.Server.Controllers
{
    public class CuratedRecipesResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public List<CuratedRecipeDTO> Recipes { get; set; } = new();
    }

    /// <summary>
    /// Controller class for all recipe related endpoints. This includes fetching recipes, adding recipes, deleting recipes, and updating recipes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        readonly RecipeService _service;

        public RecipeController(RecipeService recipeService)
        {
            _service = recipeService;
        }

        /// <summary>
        /// Used for the display of recipes in the initial state of the app. 
        /// *Not used for the user's personal recipe collection, but rather to show the user what the app is capable of.
        /// </summary>
        /// <returns>Either a list of the placeholder recipes, OR exception message</returns>
        [HttpGet("placeholders")]
        public IActionResult FetchPlaceholders()
        {
            try
            {
                var queryResult = _service.GetAllRecipes();
                List<RecipeDTO> placeholders = ToDTO(queryResult);
                return Ok(placeholders);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Returns recipes ranked by how well the authenticated user's inventory matches each recipe's raw ingredient list.
        /// </summary>
        /// <param name="limit">Maximum number of recipes to return.</param>
        /// <param name="minimumMatchPercentage">Minimum percentage match required for a recipe to be returned.</param>
        /// <returns>Curated recipes with match details.</returns>
        [Authorize]
        [HttpGet("curated")]
        public IActionResult FetchCuratedRecipes(
            [FromQuery] int limit = 25,
            [FromQuery] double minimumMatchPercentage = 0)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new CuratedRecipesResponse
                {
                    Success = false,
                    Message = "Authentication required."
                });
            }

            try
            {
                var result = _service.GetCuratedRecipesForUser(userId, limit, minimumMatchPercentage);
                //var result = _service.ComplicatedCuration(userId, limit, minimumMatchPercentage);

                return Ok(new CuratedRecipesResponse
                {
                    Success = true,
                    Message = result.Count > 0
                        ? "Curated recipes fetched successfully."
                        : "No recipe matches found yet. Add more ingredients to your inventory to improve matching.",
                    Recipes = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CuratedRecipesResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Recipes = new List<CuratedRecipeDTO>()
                });
            }
        }

        /// <summary>
        /// Private helper method to turn db returned item into a recipeDTO
        /// </summary>
        /// <param name="response">Recipe List returned from the db</param>
        /// <returns>Returns new list of RecipeDTO </returns>
        private List<RecipeDTO> ToDTO(List<Recipe> response)
        {
            List<RecipeDTO> newList = new List<RecipeDTO>();

            foreach (var item in response)
            {
                newList.Add(_service.ToDTO(item));
            }

            return newList;
        }
    }
}
