using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Services.Interfaces;

public interface IItemService
{
    Task<Item> GetById(Guid id);
    Task<ICollection<Item>> GetAllCheckItems(Guid checkId);
    Task<PagedResultDto<Item>> GetAllUserItems(string userName, AllDayExpensesRequestDto request);
    Task<RecommendationsPagedResultDto<RecommendationItemDto>> GetAllItemsForRecommendations(string userName, AllDayExpensesRequestDto request);
    Task<ICollection<string>> GetAllDistinctTags();
    Task<decimal> AddItem(Item item);
    Task<decimal> EditItem(Item item);
    Task<decimal> DeleteItem(Guid id);
    Task UpdateItemRatingAndTags(Guid itemId, int rating, ICollection<string> tags);
}
