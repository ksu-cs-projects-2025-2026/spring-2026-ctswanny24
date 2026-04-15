using Recip_EZ.Server.Data;
using Recip_EZ.Server.Models;

namespace Recip_EZ.Server.Services
{
    public class InventoryService
    {
            private readonly RecipEzDbContext _context;

            public InventoryService(RecipEzDbContext context)
            {
                _context = context;
            }

        public List<Ingredient> GetUserInventory()
        {
            List<Ingredient> inventory = _context.Ingredients.ToList();

            if (!inventory.Any())
            {
                throw new Exception("Inventory Not Found");
            }
            else
            {
                return inventory;
            }
        }

    }
}
