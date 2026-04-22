using Microsoft.AspNetCore.Mvc;
using Recip_EZ.Server.DTOs;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using System.Text.Json;

namespace Recip_EZ.Server.Controllers
{
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
