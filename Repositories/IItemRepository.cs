using ExpensesCalculator.Models;

namespace ExpensesCalculator.Repositories
{
    public interface IItemRepository
    {
        public Task<IEnumerable<Item>> GetAll(int checkId);
        public Task<Item> GetById(int id);
        public Task Insert(Item item, int checkId);
        public Task Update(Item item, int checkId);
        public Task Delete(int id, int checkId);
    }
}
