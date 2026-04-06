using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using ExpensesCalculator.WebAPI.Services.Interfaces;

namespace ExpensesCalculator.WebAPI.Services;

public class RecommendationService : IRecommendationService
{
    private readonly IItemRepository _itemRepository;
    private readonly ICheckRepository _checkRepository;

    public RecommendationService(
        IItemRepository itemRepository,
        ICheckRepository checkRepository)
    {
        _itemRepository = itemRepository;
        _checkRepository = checkRepository;
    }

    public async Task<PagedResultDto<RecommendationItemDto>> GetAllItemsForRecommendations(string userName, AllItemsRequestDto request)
    {
        return await _itemRepository.GetAllItemsForRecommendations(userName, request);
    }

    public async Task<string[]> GetAllDistinctTags(string userName)
    {
        return await _itemRepository.GetAllDistinctTags(userName);
    }

    public async Task AddRecommendationItem(string userName, CreateRecommendationItemRequestDto itemDto)
    {
        // Get or create the recommendation check for this user
        var checkId = await _itemRepository.GetOrCreateRecommendationCheckId(userName);

        // Create the item
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = itemDto.Name,
            Comment = itemDto.Comment,
            Price = itemDto.Price,
            Amount = itemDto.Amount,
            Rating = itemDto.Rating,
            Tags = itemDto.Tags ?? Array.Empty<string>(),
            Users = new[] { userName },
            CheckId = checkId
        };

        await _itemRepository.Insert(item);
    }

    public async Task EditRecommendationItem(string userName, EditRecommendationItemRequestDto itemDto)
    {
        var userId = await _itemRepository.GetUserIdByUsername(userName);

        if (userId == Guid.Empty)
            throw new KeyNotFoundException($"User {userName} not found");

        // Get the item
        var item = await _itemRepository.GetById(itemDto.Id);
        if (item == null)
            throw new KeyNotFoundException($"Item {itemDto.Id} not found");

        // Verify the item belongs to the user's recommendation check
        var check = await _checkRepository.GetById(item.CheckId);
        if (check == null || check.DayExpensesId != userId)
            throw new UnauthorizedAccessException("You can only edit your own recommendation items");

        // Update the item
        item.Name = itemDto.Name;
        item.Comment = itemDto.Comment;
        item.Price = itemDto.Price;
        item.Amount = itemDto.Amount;
        item.Rating = itemDto.Rating;
        item.Tags = itemDto.Tags ?? Array.Empty<string>();

        await _itemRepository.Update(item);
    }

    public async Task DeleteRecommendationItem(string userName, Guid itemId)
    {
        var userId = await _itemRepository.GetUserIdByUsername(userName);

        if (userId == Guid.Empty)
            throw new KeyNotFoundException($"User {userName} not found");

        // Get the item
        var item = await _itemRepository.GetById(itemId);
        if (item == null)
            throw new KeyNotFoundException($"Item {itemId} not found");

        // Verify the item belongs to the user's recommendation check
        var check = await _checkRepository.GetById(item.CheckId);
        if (check == null || check.DayExpensesId != userId)
            throw new UnauthorizedAccessException("You can only delete your own recommendation items");

        // Delete the item
        await _itemRepository.Delete(itemId);
    }
}
