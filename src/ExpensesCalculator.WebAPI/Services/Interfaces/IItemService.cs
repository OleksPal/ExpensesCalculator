using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Services.Interfaces;

public interface IItemService
{
    Task<ItemDto?> GetById(Guid id);
    Task<Item[]> GetAllCheckItems(Guid checkId);
    Task<ItemUpdateResponseDto> AddItem(CreateItemRequestDto item);
    Task<ItemUpdateResponseDto> EditItem(EditItemRequestDto item);
    Task<DeleteItemResponse> DeleteItem(Guid id);
}
