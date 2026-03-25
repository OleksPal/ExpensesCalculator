using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using ExpensesCalculator.WebAPI.Services.Interfaces;

namespace ExpensesCalculator.WebAPI.Services;

public class DayExpensesService : IDayExpensesService
{
    private readonly IDayExpensesRepository _dayExpensesRepository;
    private readonly ICheckRepository _checkRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IExpensesCalculator _expensesCalculator;

    public DayExpensesService(
        IDayExpensesRepository dayExpensesRepository,
        ICheckRepository checkRepository,
        IItemRepository itemRepository,
        IExpensesCalculator expensesCalculator)
    {
        _dayExpensesRepository = dayExpensesRepository;
        _checkRepository = checkRepository;
        _itemRepository = itemRepository;
        _expensesCalculator = expensesCalculator;
    }

    public async Task<PagedResultWithDateRangeDto<DayExpensesResponseDto>> GetAllDays(string userName, AllDayExpensesRequestDto request)
    {
        if (request.PageSize <= 0 || request.PageSize > 100)
            request.PageSize = 10;

        if (request.PageNumber <= 0)
            request.PageNumber = 1;

        var days = await _dayExpensesRepository.GetAll(userName, request);

        var dtos = days.Items.Select(day => new DayExpensesResponseDto
        {
            Id = day.Id,
            Date = day.Date,
            Participants = day.Participants,
            Location = day.Location,
            TotalSum = day.TotalSum
        }).ToArray();

        var pagedResult = new PagedResultWithDateRangeDto<DayExpensesResponseDto>
        {
            Items = dtos,
            TotalPages = days.TotalPages,
            FromDate = days.FromDate,
            ToDate = days.ToDate
        };

        return pagedResult;
    }

    public async Task<DayExpensesResponseDto?> GetById(Guid id, string userName)
    {
        var dayExpenses = await _dayExpensesRepository.GetById(id, userName);

        if (dayExpenses is null)
            return null;

        var dto = new DayExpensesResponseDto
        {
            Id = dayExpenses.Id,
            Date = dayExpenses.Date,
            Participants = dayExpenses.Participants,
            Location = dayExpenses.Location,
            TotalSum = dayExpenses.TotalSum
        };

        return dto;
    }

    public async Task<DayExpensesDetailsResponseDto?> GetDayExpensesDetails(Guid id, string userName)
    {
        var dayExpenses = await _dayExpensesRepository.GetById(id, userName);

        if (dayExpenses is null)
            return null;

        var checks = await _checkRepository.GetAllDayChecks(id);
        var checkDtos = checks.Select(check => new CheckDto
        {
            Id = check.Id,
            Location = check.Location,
            Payer = check.Payer,
            Photo = check.Photo,
            DayExpensesId = check.DayExpensesId
        }).ToList();

        for (int i = 0; i < checkDtos.Count; i++)
        {
            var items = await _itemRepository.GetAllCheckItems(checkDtos[i].Id);
            checkDtos[i].TotalSum = items.Select(item => item.Price * item.Amount).Sum();
        }

        var dto = new DayExpensesDetailsResponseDto
        {
            Id = dayExpenses.Id,
            Date = dayExpenses.Date,
            Participants = dayExpenses.Participants,
            Location = dayExpenses.Location,
            TotalSum = dayExpenses.TotalSum,
            Checks = checkDtos.ToArray()
        };

        return dto;
    }

    public async Task<DayExpensesResponseDto> AddDayExpenses(CreateDayExpensesRequestDto dayExpensesRequestDto, string userName)
    {
        // Filter out empty participant names
        dayExpensesRequestDto.Participants = dayExpensesRequestDto.Participants
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .ToList();

        if (dayExpensesRequestDto.Participants.Count == 0)
            throw new ArgumentException("At least one participant is required.");

        var dayExpenses = new DayExpenses
        {
            Date = dayExpensesRequestDto.Date,
            Participants = dayExpensesRequestDto.Participants.ToArray(),
            PeopleWithAccess = [userName],
            Location = dayExpensesRequestDto.Location,
            TotalSum = 0m
        };

        var insertedId = await _dayExpensesRepository.Insert(dayExpenses);

        return new DayExpensesResponseDto
        {
            Id = insertedId,
            Date = dayExpenses.Date,
            Participants = dayExpenses.Participants,
            Location = dayExpenses.Location,
            TotalSum = dayExpenses.TotalSum
        };
    }

    public async Task<DayExpensesResponseDto> EditDayExpenses(EditDayExpensesRequestDto dayExpensesRequestDto, string userName)
    {
        // Filter out empty participant names
        dayExpensesRequestDto.Participants = dayExpensesRequestDto.Participants
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .ToList();

        if (dayExpensesRequestDto.Participants.Count == 0)
            throw new ArgumentException("At least one participant is required.");

        var dayExpenses = await _dayExpensesRepository.GetById(dayExpensesRequestDto.Id, userName);

        if (dayExpenses is null)
            throw new KeyNotFoundException($"Day expenses with id {dayExpensesRequestDto.Id} not found.");

        dayExpenses.Date = dayExpensesRequestDto.Date;
        dayExpenses.Participants = dayExpensesRequestDto.Participants.ToArray();
        dayExpenses.Location = dayExpensesRequestDto.Location;

        await _dayExpensesRepository.Update(dayExpenses);

        return new DayExpensesResponseDto
        {
            Id = dayExpenses.Id,
            Date = dayExpenses.Date,
            Participants = dayExpenses.Participants,
            Location = dayExpenses.Location,
            TotalSum = dayExpenses.TotalSum
        };
    }

    public async Task DeleteDayExpenses(Guid id, string userName)
    {
        var dayExpenses = await _dayExpensesRepository.GetById(id, userName);

        if (dayExpenses is null)
            throw new KeyNotFoundException($"Day expenses with id {id} not found.");

        // Get all checks for this day expenses
        var checks = await _checkRepository.GetAllDayChecks(id);

        // Delete all items for each check, then delete the checks
        foreach (var check in checks)
        {
            var items = await _itemRepository.GetAllCheckItems(check.Id);
            foreach (var item in items)
            {
                await _itemRepository.Delete(item.Id);
            }
            await _checkRepository.Delete(check.Id);
        }

        // Finally delete the day expenses
        await _dayExpensesRepository.Delete(dayExpenses);
    }

    public async Task<ShareDayExpensesResponseDto> ShareDayExpenses(Guid id, string newUserWithAccess, string userName)
    {
        if (string.IsNullOrWhiteSpace(newUserWithAccess))
            return new ShareDayExpensesResponseDto(IsSuccess: false, Error: "Username is required.");

        var dayExpenses = await _dayExpensesRepository.GetById(id, userName);

        if (dayExpenses is null)
            throw new KeyNotFoundException($"Day expenses with id {id} not found.");

        if (dayExpenses.PeopleWithAccess.Contains(newUserWithAccess))
            return new ShareDayExpensesResponseDto(IsSuccess: false, Error: $"{newUserWithAccess} already has access.");

        var peopleWithAccessList = dayExpenses.PeopleWithAccess.ToList();
        peopleWithAccessList.Add(newUserWithAccess);
        dayExpenses.PeopleWithAccess = peopleWithAccessList.ToArray();

        await _dayExpensesRepository.Update(dayExpenses);

        return new ShareDayExpensesResponseDto(IsSuccess: true, Error: String.Empty);
    }

    public async Task<DayExpensesCalculationsDto> GetDayExpensesCalculations(Guid id, string userName)
    {
        var dayExpenses = await GetById(id, userName);

        if (dayExpenses is null)
            throw new KeyNotFoundException($"Day expenses with id {id} not found.");

        return await _expensesCalculator.GetCalculations(dayExpenses);
    }    
}
