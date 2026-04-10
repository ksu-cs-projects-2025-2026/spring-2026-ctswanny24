using Microsoft.AspNetCore.Mvc;
using Recip_EZ.Server.DTOs;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using System.Text.Json;

namespace Recip_EZ.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        readonly RecipeService _recipeService;

        public RecipeController(RecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet("placeholders")]
        public IActionResult FetchPlaceholders()
        {
            try
            {
                var queryResult = _recipeService.GetFirstFiveRecipes();
                List<RecipeDTO> placeholders = ToDTO(queryResult);
                return Ok(placeholders);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        private List<RecipeDTO> ToDTO(List<Recipe> response)
        {
            List<RecipeDTO> newList = new List<RecipeDTO>();

            foreach (var item in response)
            {
                newList.Add(new RecipeDTO
                {
                    RecipeId = item.RecipeId,
                    RecipeName = item.RecipeName,
                    Ingredients = JsonSerializer.Deserialize<List<string>>(item.Ingredients) ?? new List<string>(),
                    Instructions = JsonSerializer.Deserialize<List<string>>(item.Instructions) ?? new List<string>(),
                    URL = item.URL,
                    Source = item.Source,
                    RawIngredientList = JsonSerializer.Deserialize<List<string>>(item.RawIngredientList) ?? new List<string>()
                });
            }

            return newList;
        }
    }
}
