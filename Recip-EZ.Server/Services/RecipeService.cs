using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Recip_EZ.Server.Data;
using Recip_EZ.Server.DTOs;
using Recip_EZ.Server.Models;
using System.Text.Json;

namespace Recip_EZ.Server.Services
{
    public class RecipeService
    {
        private readonly RecipEzDbContext _context;

        /// <summary>
        /// Creates the RecipeService Layer. Where the DbContext is stored and can be used
        /// </summary>
        /// <param name="context">Db Context</param>
        public RecipeService(RecipEzDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to get ALL recipes in the Database (If for some reason you need to do that)
        /// </summary>
        /// <returns>List of ALL recipe items</returns>
        /// <exception cref="Exception">No recipes found exception</exception>
        public List<Recipe> GetAllRecipes()
        {
            List<Recipe> recipes = _context.Recipes.ToList();

            if(!recipes.Any() || recipes.Count == 0)
            {
                throw new Exception("No Recipes found in the database.");
            }
            else
            {
                return recipes;
            }
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

            if (!recipes.Any() || recipes.Count == 0)
            {
                throw new Exception("No Recipes found in the database.");
            }
            else
            {
                return recipes;
            }

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

        /// <summary>
        /// Grabs a recipe by its ID.
        /// </summary>
        /// <param name="id">Id of the wanted recipe</param>
        /// <returns>The recipe with the specified ID</returns>
        /// <exception cref="Exception">Thrown when no recipe is found with the specified ID</exception>
        public Recipe GetRecipeById(int id)
        {
            Recipe? recipe = _context.Recipes.FirstOrDefault(i => i.RecipeId == id);

            if(recipe == null)
            {
                throw new Exception($"No Recipe found with ID: {id}");
            }
            else
            {
                return recipe;
            }
        }


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
            return (new RecipeDTO
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

        //public void populateRecipeIngredients()
        //{
        //    var recipes = _context.Recipes.ToList();
        //    var ingredients = _context.Ingredients.ToList();

        //    foreach(var recipe in recipes)
        //    {
        //        List<string> rawIngredients = JsonSerializer.Deserialize<List<string>>(recipe.RawIngredientList) ?? new List<string>();

        //        foreach(var item in rawIngredients)
        //        {

        //            var match = ingredients.FirstOrDefault(i => Normalize(i.Name) == item);

        //            if(match != null)
        //            {
        //                _context.RecipeIngredients.Add(new RecipeIngredient
        //                {
        //                    RecipeId = recipe.RecipeId,
        //                    IngredientId = match.IngredientId
        //                });
        //            }
        //            else
        //            {
        //                Console.WriteLine($"No match found for: {match}");
        //            }
        //        }
        //    }

        //    _context.SaveChanges();
        //}


    }
}
