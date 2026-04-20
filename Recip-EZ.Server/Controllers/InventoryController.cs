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


    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        readonly InventoryService _service;

        public InventoryController(InventoryService service)
        {
            _service = service;
        }

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

            bool result = _service.AddItem(inventoryItem);
            if (result)
            {
                return Ok(new InventoryResponse(){ Success = true, Message = "Item added to inventory" });
            }
            else
            {
                return BadRequest(new InventoryResponse() { Success = false, Message = "Something went wrong. Item has NOT been added" });
            }
        }

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

    }
}
