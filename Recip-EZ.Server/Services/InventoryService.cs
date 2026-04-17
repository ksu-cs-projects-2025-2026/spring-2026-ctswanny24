using Microsoft.AspNetCore.Mvc;
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

        public bool AddItem(UserInventory item)
        {
            try
            {
                _context.UserInventories.Add(item);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Ingredient> GetIngredients()
        {
            List<Ingredient> inventory = _context.Ingredients.Take(5).ToList();

            return inventory;
        }

    }
}
