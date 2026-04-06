using ExpensesCalculator.WebAPI.Data;
using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.WebAPI.Repositories;

public class ItemRepository : GenericRepository<Item>, IItemRepository
{
    public ItemRepository(ExpensesContext context) : base(context) { }

    public async Task<Item[]> GetAllCheckItems(Guid checkId)
    {
        return await _context.Items
            .AsNoTracking()
            .Where(i => i.CheckId == checkId)
            .ToArrayAsync();
    }

    public async Task<PagedResultDto<RecommendationItemDto>> GetAllItemsForRecommendations(string userName, AllItemsRequestDto request)
    {
        var userId = await GetUserIdAsync(userName);

        if (userId == Guid.Empty)
        {
            return new PagedResultDto<RecommendationItemDto>
            {
                Items = Array.Empty<RecommendationItemDto>(),
                TotalPages = 0,
                TotalCount = 0
            };
        }

        // Get user's personal recommendation check ID in a single query
        var userCheckId = await _context.Checks
            .Where(c => c.DayExpensesId == userId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        IQueryable<Item> queryUserItems;

        if (request.IsOnlyMyItems)
        {
            // Only user's personal recommendation items
            queryUserItems = _context.Items.Where(i => i.CheckId == userCheckId);
        }
        else
        {
            // Get all check IDs from accessible day expenses (excluding dummy records) + user's personal check
            // Combined in a single query with Join
            var accessibleCheckIds = await _context.Days
                .Where(d => d.PeopleWithAccess.Contains(userName) && d.Location != "EXPENSES_CALCULATOR_RECOMMENDATIONS")
                .Join(_context.Checks,
                    day => day.Id,
                    check => check.DayExpensesId,
                    (day, check) => check.Id)
                .ToArrayAsync();

            // Include items from both user's personal check AND accessible checks
            queryUserItems = _context.Items
                .Where(i => i.CheckId == userCheckId || accessibleCheckIds.Contains(i.CheckId));
        }

        // Apply tag filtering if tags array is provided
        if (request.Tags != null && request.Tags.Length > 0)
        {
            foreach (var tag in request.Tags)
            {
                var tagLower = tag.ToLower().Trim();
                queryUserItems = queryUserItems.Where(i => i.Tags.Any(t => t.ToLower() == tagLower));
            }
        }

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
                    queryUserItems = queryUserItems.Where(i => i.Tags.Any(t => t.ToLower().Contains(tag)));
                }
            }
            else
            {
                queryUserItems = request.FilterCriteria?.ToLower() switch
                {
                    "name" => queryUserItems.Where(i => i.Name.ToLower().Contains(filterText)),
                    "description" => queryUserItems.Where(i => i.Comment != null && i.Comment.ToLower().Contains(filterText)),
                    "price" => queryUserItems.Where(i => i.Price.ToString().Contains(filterText)),
                    "amount" => queryUserItems.Where(i => i.Amount.ToString().Contains(filterText)),
                    "rating" => queryUserItems.Where(i => i.Rating.ToString().Contains(filterText)),
                    _ => queryUserItems.Where(i => i.Name.ToLower().Contains(filterText))
                };
            }
        }

        // Count total items before pagination
        var totalCount = await queryUserItems.CountAsync();

        // Apply sorting
        var isAscending = request.SortOrder?.ToLower() == "asc";
        queryUserItems = request.SortColumn?.ToLower() switch
        {
            "price" => isAscending ? queryUserItems.OrderBy(i => i.Price) : queryUserItems.OrderByDescending(i => i.Price),
            "amount" => isAscending ? queryUserItems.OrderBy(i => i.Amount) : queryUserItems.OrderByDescending(i => i.Amount),
            "rating" => isAscending ? queryUserItems.OrderBy(i => i.Rating) : queryUserItems.OrderByDescending(i => i.Rating),
            "totalprice" => isAscending ? queryUserItems.OrderBy(i => i.Price * i.Amount) : queryUserItems.OrderByDescending(i => i.Price * i.Amount),
            "usercount" => isAscending ? queryUserItems.OrderBy(i => i.Users.Count) : queryUserItems.OrderByDescending(i => i.Users.Count),
            _ => isAscending ? queryUserItems.OrderBy(i => i.Name) : queryUserItems.OrderByDescending(i => i.Name)
        };

        // Apply pagination and project to DTO
        var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        var items = await queryUserItems
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Join(_context.Checks,
                item => item.CheckId,
                check => check.Id,
                (item, check) => new { Item = item, Check = check })
            .Select(joined => new RecommendationItemDto
            {
                Id = joined.Item.Id,
                Name = joined.Item.Name,
                Comment = joined.Item.Comment,
                Price = joined.Item.Price,
                Amount = joined.Item.Amount,
                Rating = joined.Item.Rating,
                Tags = joined.Item.Tags,
                Users = joined.Item.Users,
                CheckId = joined.Item.CheckId,
                DayExpensesId = joined.Check.DayExpensesId,
                CanEdit = joined.Item.CheckId == userCheckId
            })
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResultDto<RecommendationItemDto>
        {
            Items = items.ToArray(),
            TotalPages = totalPages,
            TotalCount = totalCount
        };
    }

    public async Task<string[]> GetAllDistinctTags(string userName)
    {
        // Get accessible day IDs first (small result set) - filter early to reduce join size
        var accessibleDayIds = await _context.Days
            .AsNoTracking()
            .Where(d => d.PeopleWithAccess.Contains(userName))
            .Select(d => d.Id)
            .ToArrayAsync();

        // Then get tags only from items in those days
        return await _context.Items
            .AsNoTracking()
            .Join(_context.Checks,
                item => item.CheckId,
                check => check.Id,
                (item, check) => new { item.Tags, check.DayExpensesId })
            .Where(ic => accessibleDayIds.Contains(ic.DayExpensesId))
            .SelectMany(x => x.Tags)
            .Distinct()
            .OrderBy(t => t)
            .ToArrayAsync();
    }

    public async Task<Guid> GetUserIdByUsername(string userName)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName);
        return user?.Id ?? Guid.Empty;
    }

    public async Task<Guid> GetOrCreateRecommendationCheckId(string userName)
    {
        var userId = await GetUserIdByUsername(userName);

        if (userId == Guid.Empty)
            throw new KeyNotFoundException($"User {userName} not found");

        // Find or create recommendation DayExpenses with Id = userId
        var dayExpenses = await _context.Days
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == userId);

        if (dayExpenses == null)
        {
            dayExpenses = new DayExpenses
            {
                Id = userId,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Participants = new[] { userName },
                PeopleWithAccess = new[] { userName },
                Location = "EXPENSES_CALCULATOR_RECOMMENDATIONS",
                TotalSum = 0
            };
            await _context.Days.AddAsync(dayExpenses);
            await _context.SaveChangesAsync();
        }

        // Find or create Check for this DayExpenses
        var check = await _context.Checks
            .FirstOrDefaultAsync(c => c.DayExpensesId == userId);

        if (check == null)
        {
            check = new Check
            {
                Id = Guid.NewGuid(),
                Location = "EXPENSES_CALCULATOR_RECOMMENDATIONS",
                Payer = userName,
                DayExpensesId = userId
            };
            await _context.Checks.AddAsync(check);
            await _context.SaveChangesAsync();
        }

        return check.Id;
    }

    private async Task<Guid> GetUserIdAsync(string userName)
    {
        return await GetUserIdByUsername(userName);
    }
}
