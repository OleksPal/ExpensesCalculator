using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Repositories
{
    public class ItemRepository : GenericRepository<Item>, IItemRepository
    {
        public ItemRepository(ExpensesContext context) : base(context) { }

        public async Task<ICollection<Item>> GetAllCheckItems(int checkId)
        {
            var items = await _context.Items.Where(i => i.CheckId == checkId).ToListAsync();

            return (items is not null) ? items : new List<Item>();
        }
    }
}
