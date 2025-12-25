using ExpensesCalculator.WebAPI.Helpers;
using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using ExpensesCalculator.WebAPI.Services.Interfaces;

namespace ExpensesCalculator.WebAPI.Services;

public class DayExpensesService : IDayExpensesService
{
    private readonly IDayExpensesRepository _dayExpensesRepository;
    private readonly ICheckRepository _checkRepository;
    private readonly ICheckService _checkService;

    public DayExpensesService(
        IDayExpensesRepository dayExpensesRepository,
        ICheckRepository checkRepository,
        ICheckService checkService
    )
    {
        _dayExpensesRepository = dayExpensesRepository;
        _checkRepository = checkRepository;
        _checkService = checkService;
    }

    public async Task<PagedResultWithDateRangeDto<DayExpensesResponseDto>> GetAllDays(string userName, AllDayExpensesRequestDto request)
    {
        var days = await _dayExpensesRepository.GetAll(userName, request);

        var dtos = days.Items.Select(day => new DayExpensesResponseDto
        {
            Id = day.Id,
            Date = day.Date,
            Participants = day.Participants,
            Location = day.Location            
        });

        foreach (var dto in dtos)
            dto.TotalSum = await GetTotalSumForDayExpensesChecks(dto.Id);     

        var isAscending = request.SortOrder.ToLower() == "asc";        
        if (request.SortColumn == "totalSum")
            dtos = isAscending ? dtos.OrderBy(dto => dto.TotalSum) : dtos.OrderByDescending(dto => dto.TotalSum);

        var sortedByDateDtos = dtos.OrderBy(dto => dto.Date);

        var pagedResult = new PagedResultWithDateRangeDto<DayExpensesResponseDto>
        {
            Items = dtos.ToArray(),
            TotalPages = days.TotalPages,
            FromDate = days.FromDate,
            ToDate = days.ToDate
        };

        return pagedResult;
    }

    public async Task<DayExpenses> GetById(Guid id, string userName)
    {
        return await _dayExpensesRepository.GetById(id, userName);
    }

    public async Task AddDayExpenses(CreateDayExpensesRequestDto dayExpensesRequestDto, string userName)
    {
        var dayExpenses = dayExpensesRequestDto.ToDayExpenses(userName);
        await _dayExpensesRepository.Insert(dayExpenses);
    }

    public async Task EditDayExpenses(EditDayExpensesRequestDto dayExpensesRequestDto, string userName)
    {
        var dayExpenses = await _dayExpensesRepository.GetById(dayExpensesRequestDto.Id, userName);
        DayExpensesHelper.UpdateDayExpenses(ref dayExpenses, dayExpensesRequestDto);
        await _dayExpensesRepository.Update(dayExpenses);
    }

    public async Task DeleteDayExpenses(Guid id, string userName)
    {
        await _dayExpensesRepository.Delete(id, userName);
    }

    public async Task<ShareDayExpensesResponseDto> ShareDayExpenses(Guid id, string newUserWithAccess, string userName)
    {
        var dayExpenses = await _dayExpensesRepository.GetById(id, userName);

        if (dayExpenses.PeopleWithAccess.Contains(newUserWithAccess))
            return new ShareDayExpensesResponseDto(IsSuccess: false, Error: $"{newUserWithAccess} already has access.");

        dayExpenses.PeopleWithAccess.Add(newUserWithAccess);
        await _dayExpensesRepository.Update(dayExpenses);

        return new ShareDayExpensesResponseDto(IsSuccess: true, Error: "");
    }

    private async Task<decimal> GetTotalSumForDayExpensesChecks(Guid dayExpensesId)
    {
        var checks = await _checkService.GetAllDayExpensesChecks(dayExpensesId);
        return checks.Select(check => check.TotalSum).Sum();
    }
}
