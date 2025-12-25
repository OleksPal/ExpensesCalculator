using ExpensesCalculator.WebAPI.Data;
using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.WebAPI.Repositories;

public class ItemRepository : GenericRepository<Item>, IItemRepository
{
    public ItemRepository(ExpensesContext context) : base(context) { }

    public async Task<ICollection<Item>> GetAllCheckItems(Guid checkId)
    {
        return await _context.Items.Where(i => i.CheckId == checkId).ToListAsync();
    }
}
