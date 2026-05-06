using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recip_EZ.Server.Data;
using Recip_EZ.Server.DTOs;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Enums;
using System.Reflection.Metadata.Ecma335;

namespace Recip_EZ.Server.Services
{
    /// <summary>
    /// Service layer for the UserInventory. 
    /// This will handle logic for fetching and manipulating the user's inventory of ingredients.
    /// </summary>
    public class InventoryService
    {
        #region Fields

        private readonly RecipEzDbContext _context;

        #endregion

        #region Constructor(s)
        /// <summary>
        /// Constructor for grabbing db context.
        /// </summary>
        /// <param name="context">The database context to be used by this service</param>
        public InventoryService(RecipEzDbContext context)
        {
            _context = context;
        }

        #endregion

        #region CRUD Methods

        /// <summary>
        /// Adds a new user inventory item to the data store.
        /// </summary>
        /// <param name="item">The user inventory item to add. Cannot be null.</param>
        /// <returns>The added user inventory item if the operation succeeds; otherwise, null.</returns>
        public UserInventory? AddItem(UserInventory item)
        {
            try
            {
                _context.UserInventories.Add(item);
                _context.SaveChanges();
                return item;
            }
            catch
            {
                return null;
            }
        }

        #region Read Operations

        /// <summary>
        /// Retrieves all ingredients from the data source.
        /// </summary>
        /// <returns>A list of <see cref="Ingredient"/> objects representing all ingredients. 
        /// The list will be empty if no ingredients are found.
        /// This is to help populate the dropdown for adding items to the inventory, so we can get the ingredient name from the id.
        /// </returns>
        public List<Ingredient> GetIngredients()
        {
            List<Ingredient> ingredients = _context.Ingredients.ToList();
            return ingredients;
        }

        /// <summary>
        /// This is the method that retrieves the user's inventory based on their user ID. 
        /// It joins the UserInventories with the Ingredients table to get the ingredient names and other details, 
        /// and returns a list of UserInventoryDTOs that can be easily used and understood in the GUI to display the user's inventory.
        /// </summary>
        /// <param name="id">UserId that identifies the user whose inventory is being retrieved.</param>
        /// <returns>A list of <see cref="UserInventoryDTO"/> objects representing the user's inventory.</returns>
        public List<UserInventoryDTO> GetInventory(int id)
        {
            var result = _context.UserInventories
                    .Where(ui => ui.UserId == id)
                    .Join(_context.Ingredients,
                        ui => ui.IngredientId,
                        i => i.IngredientId,
                        (ui, i) => new UserInventoryDTO
                        {
                            UserInventoryId = ui.UserInventoryId,
                            IngredientId = i.IngredientId,
                            IngredientName = i.Name!,
                            Quantity = ui.Quantity,
                            Unit = (Unit)ui.Unit
                        })
                    .ToList();

            return result;
        }

        #endregion

        //Placeholder method for V1.0 feature to edit an item in the user's inventory.
        public void EditItem(UserInventory item)
        {
            return;
        }

        /// <summary>
        /// Simple logic to delete the item from the user's inventory based on the UserInventoryId.
        /// </summary>
        /// <param name="id">Id of the user inventory item to be deleted.</param>
        /// <exception cref="Exception">
        /// Thrown when the item is not found.
        /// Ideally shouldn't be happening, but just in case something goes wrong, we can catch it and display an error message to the user.
        /// </exception>
        public void DeleteItem(int id, int userId)
        {
            var item = _context.UserInventories.FirstOrDefault(x => x.UserInventoryId == id && x.UserId == userId);

            if (item == null)
            {
                throw new Exception("Item not found");
            }

            _context.UserInventories.Remove(item);
            _context.SaveChanges();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Easy helper method to grab the ingredient name from the ingredient id. 
        /// This is used in the GUI to display the ingredient name instead of just the id.
        /// Since userInventory doesn't have a base Name property (it's just an id that we reference in the ingredients table)
        /// </summary>
        /// <param name="id">IngredientId to find the name</param>
        /// <returns>The name of the ingredient.</returns>
        /// <exception cref="Exception">Thrown when the ingredient is not found.</exception>
        public string GetIngredientName(int id)
        {
            var ingredient = _context.Ingredients.FirstOrDefault(x => x.IngredientId == id);
            if (ingredient == null)
            {
                throw new Exception("Ingredient not found");
            }
            return ingredient.Name!;
        }
        #endregion
    }
}
