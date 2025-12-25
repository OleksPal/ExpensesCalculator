using ExpensesCalculator.WebAPI.Data;
using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.WebAPI.Repositories;

public class DayExpensesRepository : IDayExpensesRepository
{
    protected readonly ExpensesContext _context;

    public DayExpensesRepository(ExpensesContext context)
    {
        _context = context;
    }    

    public async Task<PagedResultWithDateRangeDto<DayExpenses>> GetAll(string userName, AllDayExpensesRequestDto request)
    {
        if (request.PageSize <= 0 || request.PageSize > 100)
            request.PageSize = 10;

        if (request.PageNumber <= 0)
            request.PageNumber = 1;

        var query = _context.Days
            .Where(day => day.PeopleWithAccess.Contains(userName));

        query = ApplyTextFilter(query, request.FilterCriteria, request.FilterText);
        query = ApplySorting(query, request.SortColumn, request.SortOrder);        

        if (request.FromDate.HasValue)
            query = query.Where(day => day.Date >= request.FromDate);

        if (request.ToDate.HasValue)
            query = query.Where(day => day.Date <= request.ToDate);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var days = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        if (!request.FromDate.HasValue)
            request.FromDate = await _context.Days
                .Where(day => day.PeopleWithAccess.Contains(userName))
                .MinAsync(day => day.Date);

        if (!request.ToDate.HasValue)
            request.ToDate = await _context.Days
                .Where(day => day.PeopleWithAccess.Contains(userName))
                .MaxAsync(day => day.Date);

        return new PagedResultWithDateRangeDto<DayExpenses>
        {
            Items = days,
            TotalPages = totalPages,
            FromDate = request.FromDate,
            ToDate = request.ToDate
        };
    }

    private static IQueryable<DayExpenses> ApplyTextFilter(IQueryable<DayExpenses> query, string? filterCriteria, string? filterText)
    {
        if (string.IsNullOrWhiteSpace(filterText) || string.IsNullOrWhiteSpace(filterCriteria))
            return query;

        var normalizedText = filterText.Trim().ToLower();
        var normalizedCriteria = filterCriteria.Trim().ToLower();

        return normalizedCriteria switch
        {
            "location" => query.Where(day =>
                day.Location != null &&
                day.Location.ToLower().Contains(normalizedText)),

            "participants" => query.Where(day =>
                day.Participants.Any(p => p.ToLower().Contains(normalizedText)) ||
                day.Participants.Count.ToString() == normalizedText),

            _ => query
        };
    }

    private static IQueryable<DayExpenses> ApplySorting(IQueryable<DayExpenses> query, string? sortColumn, string? sortOrder)
    {
        var isAscending = sortOrder?.ToLower() == "asc";
        var column = sortColumn?.ToLower() ?? "date";

        return column switch
        {
            "date" => isAscending
                ? query.OrderBy(day => day.Date)
                : query.OrderByDescending(day => day.Date),

            "location" => isAscending
                ? query.OrderBy(day => day.Location)
                : query.OrderByDescending(day => day.Location),

            "participants" => isAscending
                ? query.OrderBy(day => day.Participants.Count)
                : query.OrderByDescending(day => day.Participants.Count),

            _ => query.OrderByDescending(day => day.Date)
        };
    }

    public async Task<DayExpenses> GetById(Guid id, string userName)
    {
        return await _context.Days.FirstOrDefaultAsync(day => day.Id == id && day.PeopleWithAccess.Contains(userName));
    }

    public async Task Insert(DayExpenses dayExpenses)
    {
        await _context.Days.AddAsync(dayExpenses);
        await _context.SaveChangesAsync();
    }

    public async Task Update(DayExpenses dayExpenses)
    {
        _context.Days.Update(dayExpenses);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id, string userName)
    {
        var dayExpenses = await _context.Days.FirstOrDefaultAsync(day => day.Id == id && day.PeopleWithAccess.Contains(userName));

        if (dayExpenses is not null)
        {
            _context.Days.Remove(dayExpenses);
            await _context.SaveChangesAsync();
        }
    }
}
