using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Recip_EZ.Server.DTOs;
using Recip_EZ.Server.Enums;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using System.Diagnostics.Contracts;

namespace Recip_EZ.Server.Controllers
{
    public class InventoryPayload
    {
        public required int UserId { get; set; }

        public required int IngredientId { get; set; }

        public required float Quantity { get; set; }

        public required string Unit { get; set; }
    }

    public class InventoryResponse
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public List<UserInventoryDTO>? Inventory { get; set; }
    }


    /// <summary>
    /// Controller for all Inventory related endpoints. 
    /// This will handle all requests related to the user's inventory of ingredients, such as adding items, fetching the inventory list, and deleting items from the inventory.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        #region Fields

        readonly InventoryService _service;

        #endregion

        #region Constructor(s)

        public InventoryController(InventoryService service)
        {
            _service = service;
        }

        #endregion

        #region Endpoints

        /// <summary>
        /// Gets the ingredients list to populate the dropdown menu in the frontend when adding an ingredient to the inventory.
        /// </summary>
        /// <returns>List of ingredients</returns>
        [HttpGet("ingredients")]
        public IActionResult PopulateIngredientList()
        {
            try
            {
                var result = _service.GetIngredients();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Logic to add an ingredient to the user's inventory.
        /// </summary>
        /// <param name="item">The inventory item to add</param>
        /// <returns>Action result indicating success or failure</returns>
        [HttpPost("add")]
        public IActionResult AddIngredient([FromBody] InventoryPayload item)
        {
            UserInventory inventoryItem = new UserInventory()
            {
                UserId = item.UserId,
                IngredientId = item.IngredientId,
                Unit = Enum.Parse<Unit>(item.Unit),
                Quantity = item.Quantity,
                DateAdded = DateTime.UtcNow
            };

            UserInventory result = _service.AddItem(inventoryItem);
            if (result == null)
            {
                return BadRequest(new InventoryResponse() { Success = false, Message = "Something went wrong. Item has NOT been added" });
            }
            else
            {
                var dto = new UserInventoryDTO()
                {

                    UserInventoryId = result.UserInventoryId,
                    UserId = result.UserId,
                    IngredientId = result.IngredientId,
                    IngredientName = _service.GetIngredientName(result.IngredientId),
                    Unit = (Unit)result.Unit,
                    Quantity = result.Quantity,
                    DateAdded = result.DateAdded,
                    ExpirationDate = result.ExpirationDate
                };

                return Ok(new InventoryResponse(){ Success = true, Message = "Item added to inventory", Inventory = new List<UserInventoryDTO>() { dto } });
            }
        }

        /// <summary>
        /// Fetches the user's inventory list, which includes all the ingredients they have added to their inventory, along with details such as quantity, unit, and expiration date (if added).
        /// </summary>
        /// <param name="userId">The ID of the user whose inventory is being fetched</param>
        /// <returns>Action result containing the user's inventory</returns>
        [HttpGet("userInventory")]
        public IActionResult FetchUserInventory([FromQuery] int userId)
        {
            try
            {
                var result = _service.GetInventory(userId);
                return Ok(new InventoryResponse() { Success = true, Message = "Ingredients successfully fetched", Inventory = result});
            }
            catch (Exception ex)
            {
                return NotFound(new InventoryResponse() { Success = false, Message = $"Failed to fetch Ingredients: Exception: {ex.Message}" });
            }
        }


        /// <summary>
        /// Deletes the specified inventory item from the user's inventory.
        /// This is based on the id passed in through the URL, which is the UserInventoryId of the item to be deleted.
        /// </summary>
        /// <param name="id">The ID of the inventory item to delete</param>
        /// <returns>Action result indicating success or failure</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteInventoryItem(int id)
        {
            try
            {
                _service.DeleteItem(id);
                return Ok(new { Success = true, Message = "Item Deleted" });
            }
            catch(Exception ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
        }

        #endregion
    }
}
