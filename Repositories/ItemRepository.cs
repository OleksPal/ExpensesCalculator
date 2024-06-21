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

        public async Task<IEnumerable<Item>> GetAll(int checkId)
        {
            var check = await _context.Checks.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == checkId);

            return (check is not null) ? check.Items : new List<Item>();
        }

        public async Task<Item> GetById(int id)
        {
            return await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task Insert(Item item, int checkId)
        {
            var check = await _context.Checks.FindAsync(checkId);

            if (check is not null)
            {
                await _context.Items.AddAsync(item);

                if (check.Items is null)
                    check.Items = new List<Item>();

                check.Items.Add(item);
                check.Sum += item.Price;
                await _context.SaveChangesAsync();
            }
        }

        public async Task Update(Item item, int checkId)
        {
            var itemToUpdate = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == item.Id);
            var check = await _context.Checks.FindAsync(checkId);

            if (itemToUpdate is not null && check is not null) 
            {
                check.Sum -= itemToUpdate.Price;
                _context.Items.Update(item);
                check.Sum += item.Price;

                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id, int checkId)
        {
            var itemToDelete = await _context.Items.FindAsync(id);
            var check = await _context.Checks.FindAsync(checkId);

            if (itemToDelete is not null && check is not null) 
            {
                check.Sum -= itemToDelete.Price;
                _context.Remove(itemToDelete);

                await _context.SaveChangesAsync();
            }
        }
    }
}
