using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recip_EZ.Server.Data;
using Recip_EZ.Server.DTOs;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Enums;

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
            List<Ingredient> ingredients = _context.Ingredients.ToList();
            return ingredients;
        }

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
                            Unit = ui.Unit
                        })
                    .ToList();

            return result;
        }

    }
}
