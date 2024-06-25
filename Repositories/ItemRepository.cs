using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ExpensesContext _context;
        public ItemRepository(ExpensesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Item>> GetAllCheckItems(int checkId)
        {
            var items = await _context.Items.Where(i => i.CheckId == checkId).ToListAsync();

            return (items is not null) ? items : new List<Item>();
        }

        public async Task<Item> GetById(int id)
        {
            return await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Item> Insert(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<Item> Update(Item item)
        {
            var itemToUpdate = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == item.Id);

            if (itemToUpdate is not null) 
            {
                _context.Items.Update(item);
                await _context.SaveChangesAsync();
            }

            return item;
        }

        public async Task<Item> Delete(int id)
        {
            var itemToDelete = await _context.Items.FindAsync(id);

            if (itemToDelete is not null) 
            {
                _context.Items.Remove(itemToDelete);
                await _context.SaveChangesAsync();
            }

            return itemToDelete;
        }
    }
}
