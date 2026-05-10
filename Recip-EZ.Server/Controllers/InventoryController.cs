using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recip_EZ.Server.DTOs;
using Recip_EZ.Server.Enums;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using System.Security.Claims;

namespace Recip_EZ.Server.Controllers
{
    public class InventoryPayload
    {
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
        [Authorize]
        [HttpPost("add")]
        public IActionResult AddIngredient([FromBody] InventoryPayload item)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized(new InventoryResponse { Success = false, Message = "Authentication required." });
            }

            UserInventory inventoryItem = new UserInventory()
            {
                UserId = userId,
                IngredientId = item.IngredientId,
                Unit = Enum.Parse<Unit>(item.Unit),
                Quantity = item.Quantity,
                DateAdded = DateTime.UtcNow
            };

            UserInventory? result = _service.AddItem(inventoryItem);
            if (result == null)
            {
                return BadRequest(new InventoryResponse() { Success = false, Message = "Something went wrong. Item has NOT been added" });
            }

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

            return Ok(new InventoryResponse() { Success = true, Message = "Item added to inventory", Inventory = new List<UserInventoryDTO>() { dto } });
        }

        /// <summary>
        /// Fetches the authenticated user's inventory list.
        /// </summary>
        /// <returns>Action result containing the user's inventory</returns>
        [Authorize]
        [HttpGet("userInventory")]
        public IActionResult FetchUserInventory()
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized(new InventoryResponse { Success = false, Message = "Authentication required." });
            }

            try
            {
                var result = _service.GetInventory(userId);
                return Ok(new InventoryResponse() { Success = true, Message = "Ingredients successfully fetched", Inventory = result });
            }
            catch (Exception ex)
            {
                return NotFound(new InventoryResponse() { Success = false, Message = $"Failed to fetch Ingredients: Exception: {ex.Message}" });
            }
        }

        /// <summary>
        /// Deletes the specified inventory item from the authenticated user's inventory.
        /// </summary>
        /// <param name="id">The ID of the inventory item to delete</param>
        /// <returns>Action result indicating success or failure</returns>
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteInventoryItem(int id)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized(new { Success = false, Message = "Authentication required." });
            }

            try
            {
                _service.DeleteItem(id, userId);
                return Ok(new { Success = true, Message = "Item Deleted" });
            }
            catch (Exception ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
        }

        #endregion
        /// <summary>
        /// Attempts to retrieve the authenticated user's ID from the claims in the JWT token.
        /// </summary>
        /// <param name="userId">The authenticated user's ID if found, otherwise 0.</param>
        /// <returns>True if the user ID was successfully retrieved, otherwise false.</returns>
        private bool TryGetAuthenticatedUserId(out int userId)
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claimValue, out userId);
        }
    }
}
