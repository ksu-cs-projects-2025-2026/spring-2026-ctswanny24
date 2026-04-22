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

        public UserInventory AddItem(UserInventory item)
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

        public void DeleteItem(int id)
        {
            var item = _context.UserInventories.FirstOrDefault(x => x.UserInventoryId == id);

            if (item == null)
            {
                throw new Exception("Item not found");
            }

            _context.UserInventories.Remove(item);
            _context.SaveChanges();
        }

        public string GetIngredientName(int id)
        {
            var ingredient = _context.Ingredients.FirstOrDefault(x => x.IngredientId == id);
            if (ingredient == null)
            {
                throw new Exception("Ingredient not found");
            }
            return ingredient.Name!;
        }
    }
}
