using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Repositories.Interfaces;

public interface IItemRepository : IGenericRepository<Item>
{
    Task<Item[]> GetAllCheckItems(Guid checkId);
    Task<PagedResultDto<RecommendationItemDto>> GetAllItemsForRecommendations(string userName, AllItemsRequestDto request);
    Task<string[]> GetAllDistinctTags(string userName);
    Task<Guid> GetUserIdByUsername(string userName);
    Task<Guid> GetOrCreateRecommendationCheckId(string userName);
}
