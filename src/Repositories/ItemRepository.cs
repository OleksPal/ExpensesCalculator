using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Repositories
{
    public class ItemRepository : GenericRepository<Item>, IItemRepository
    {
        public ItemRepository(ExpensesContext context) : base(context) { }

        public async Task<ICollection<Item>> GetAllCheckItems(Guid checkId)
        {
            var items = await _context.Items.Where(i => i.CheckId == checkId).ToListAsync();

            return (items is not null) ? items : new List<Item>();
        }

        public override async Task<Item> GetById(Guid id)
        {
            var item = await _context.Items.Include(i => i.Check).FirstOrDefaultAsync(i => i.Id == id);

            return item;
        }

        public async Task<decimal> GetItemPriceById(Guid id)
        {
            return await _context.Items.Where(i => i.Id == id).Select(i => i.Price).SingleOrDefaultAsync();
        }
    }
}
