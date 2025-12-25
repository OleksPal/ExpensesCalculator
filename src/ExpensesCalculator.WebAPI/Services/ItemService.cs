using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using ExpensesCalculator.WebAPI.Services.Interfaces;

namespace ExpensesCalculator.WebAPI.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;

    public ItemService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Item> GetById(Guid id)
    {
        return await _itemRepository.GetById(id);
    }

    public async Task AddItem(Item item)
    {
        await _itemRepository.Insert(item);
    }

    public async Task EditItem(Item item)
    {
        await _itemRepository.Update(item);
    }

    public async Task DeleteItem(Guid id)
    {
        await _itemRepository.Delete(id);
    }
}
