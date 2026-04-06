using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Services.Interfaces;

public interface IRecommendationService
{
    Task<PagedResultDto<RecommendationItemDto>> GetAllItemsForRecommendations(string userName, AllItemsRequestDto request);
    Task<string[]> GetAllDistinctTags(string userName);
    Task AddRecommendationItem(string userName, CreateRecommendationItemRequestDto item);
    Task EditRecommendationItem(string userName, EditRecommendationItemRequestDto item);
    Task DeleteRecommendationItem(string userName, Guid itemId);
}
