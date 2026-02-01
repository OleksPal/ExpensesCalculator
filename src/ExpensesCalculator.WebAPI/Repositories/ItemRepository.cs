using ExpensesCalculator.WebAPI.Data;
using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
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

    public async Task<PagedResultDto<Item>> GetAllUserItems(string userName, AllDayExpensesRequestDto request)
    {
        // Get all items where the user has access through DayExpenses
        var query = from item in _context.Items
                    join check in _context.Checks on item.CheckId equals check.Id
                    join day in _context.Days on check.DayExpensesId equals day.Id
                    where day.PeopleWithAccess.Contains(userName)
                    select item;

        // Apply filtering
        if (!string.IsNullOrWhiteSpace(request.FilterText))
        {
            var filterText = request.FilterText.ToLower().Trim();

            if (request.FilterCriteria?.ToLower() == "tags")
            {
                // Handle pipe-separated tags with AND logic
                var searchTags = filterText.Split('|', StringSplitOptions.RemoveEmptyEntries);
                foreach (var searchTag in searchTags)
                {
                    var tag = searchTag.Trim().ToLower();
                    query = query.Where(i => i.Tags.Any(t => t.ToLower().Contains(tag)));
                }
            }
            else
            {
                query = request.FilterCriteria?.ToLower() switch
                {
                    "name" => query.Where(i => i.Name.ToLower().Contains(filterText)),
                    "description" => query.Where(i => i.Description != null && i.Description.ToLower().Contains(filterText)),
                    "price" => query.Where(i => i.Price.ToString().Contains(filterText)),
                    "amount" => query.Where(i => i.Amount.ToString().Contains(filterText)),
                    "rating" => query.Where(i => i.Rating.ToString().Contains(filterText)),
                    _ => query.Where(i => i.Name.ToLower().Contains(filterText))
                };
            }
        }

        // Count total items before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        var isAscending = request.SortOrder?.ToLower() == "asc";
        query = request.SortColumn?.ToLower() switch
        {
            "price" => isAscending ? query.OrderBy(i => i.Price) : query.OrderByDescending(i => i.Price),
            "amount" => isAscending ? query.OrderBy(i => i.Amount) : query.OrderByDescending(i => i.Amount),
            "rating" => isAscending ? query.OrderBy(i => i.Rating) : query.OrderByDescending(i => i.Rating),
            "totalprice" => isAscending ? query.OrderBy(i => i.Price * i.Amount) : query.OrderByDescending(i => i.Price * i.Amount),
            "usercount" => isAscending ? query.OrderBy(i => i.Users.Count) : query.OrderByDescending(i => i.Users.Count),
            _ => isAscending ? query.OrderBy(i => i.Name) : query.OrderByDescending(i => i.Name)
        };

        // Apply pagination
        var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResultDto<Item>
        {
            Items = items,
            TotalPages = totalPages,
            TotalCount = totalCount
        };
    }

    public async Task<RecommendationsPagedResultDto<RecommendationItemDto>> GetAllItemsForRecommendations(string userName, AllDayExpensesRequestDto request)
    {
        // Get all items - simple SELECT * FROM Items
        var query = _context.Items.AsQueryable();

        // Apply filtering
        if (!string.IsNullOrWhiteSpace(request.FilterText))
        {
            var filterText = request.FilterText.ToLower().Trim();

            if (request.FilterCriteria?.ToLower() == "tags")
            {
                // Handle pipe-separated tags with AND logic
                var searchTags = filterText.Split('|', StringSplitOptions.RemoveEmptyEntries);
                foreach (var searchTag in searchTags)
                {
                    var tag = searchTag.Trim().ToLower();
                    query = query.Where(i => i.Tags.Any(t => t.ToLower().Contains(tag)));
                }
            }
            else
            {
                query = request.FilterCriteria?.ToLower() switch
                {
                    "name" => query.Where(i => i.Name.ToLower().Contains(filterText)),
                    "description" => query.Where(i => i.Description != null && i.Description.ToLower().Contains(filterText)),
                    "price" => query.Where(i => i.Price.ToString().Contains(filterText)),
                    "amount" => query.Where(i => i.Amount.ToString().Contains(filterText)),
                    "rating" => query.Where(i => i.Rating.ToString().Contains(filterText)),
                    _ => query.Where(i => i.Name.ToLower().Contains(filterText))
                };
            }
        }

        // Count total items before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        var isAscending = request.SortOrder?.ToLower() == "asc";
        query = request.SortColumn?.ToLower() switch
        {
            "price" => isAscending ? query.OrderBy(i => i.Price) : query.OrderByDescending(i => i.Price),
            "amount" => isAscending ? query.OrderBy(i => i.Amount) : query.OrderByDescending(i => i.Amount),
            "rating" => isAscending ? query.OrderBy(i => i.Rating) : query.OrderByDescending(i => i.Rating),
            "totalprice" => isAscending ? query.OrderBy(i => i.Price * i.Amount) : query.OrderByDescending(i => i.Price * i.Amount),
            "usercount" => isAscending ? query.OrderBy(i => i.Users.Count) : query.OrderByDescending(i => i.Users.Count),
            _ => isAscending ? query.OrderBy(i => i.Name) : query.OrderByDescending(i => i.Name)
        };

        // Apply pagination and project to DTO
        var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new RecommendationItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Amount = item.Amount,
                Rating = item.Rating,
                Tags = item.Tags,
                Users = item.Users,
                CheckId = item.CheckId,
                CanEdit = true // hardcore for now
            })
            .ToListAsync();

        items.ForEach(i => i.DayExpensesId = GetCheckDayExpensesId(i.CheckId));

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new RecommendationsPagedResultDto<RecommendationItemDto>
        {
            Items = items,
            TotalPages = totalPages,
            TotalCount = totalCount,
            CurrentUserId = GetUserId(userName)
        };
    }

    public async Task<ICollection<string>> GetAllDistinctTags()
    {
        var allTags = await _context.Items
            .SelectMany(i => i.Tags)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        return allTags;
    }

    private Guid GetCheckDayExpensesId(Guid checkId)
    {
        var check = _context.Checks.Find(checkId);
        return check?.DayExpensesId ?? Guid.Empty;
    }

    private Guid GetUserId(string userName)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        return user?.Id ?? Guid.Empty;
    }
}
