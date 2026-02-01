using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Repositories.Interfaces;

public interface IItemRepository : IGenericRepository<Item>
{
    Task<ICollection<Item>> GetAllCheckItems(Guid checkId);
    Task<PagedResultDto<Item>> GetAllUserItems(string userName, AllDayExpensesRequestDto request);
    Task<RecommendationsPagedResultDto<RecommendationItemDto>> GetAllItemsForRecommendations(string userName, AllDayExpensesRequestDto request);
    Task<ICollection<string>> GetAllDistinctTags();
}
