using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Controllers
{
    public class InventoryItem
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {
        // GET: InventoryController
        // POST: InventoryController/Create
        [HttpPost]
        public IActionResult AddItem([FromBody] Models.Ingredient data)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Unauthorized();
            }
        }
    }
}
