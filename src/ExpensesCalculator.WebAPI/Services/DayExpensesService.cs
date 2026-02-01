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
    private readonly IItemRepository _itemRepository;

    public DayExpensesService(
        IDayExpensesRepository dayExpensesRepository,
        ICheckRepository checkRepository,
        ICheckService checkService,
        IItemRepository itemRepository
    )
    {
        _dayExpensesRepository = dayExpensesRepository;
        _checkRepository = checkRepository;
        _checkService = checkService;
        _itemRepository = itemRepository;
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
        }).ToList();

        for (int i = 0; i < dtos.Count(); i++)
        {
            dtos[i].TotalSum = await GetTotalSumForDayExpensesChecks(dtos[i].Id);
        } 

        var isAscending = request.SortOrder.ToLower() == "asc";
        if (request.SortColumn == "totalSum")
            dtos = isAscending ? dtos.OrderBy(dto => dto.TotalSum).ToList() : dtos.OrderByDescending(dto => dto.TotalSum).ToList();

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

    public async Task<DayExpensesResponseDto> GetById(Guid id, string userName)
    {
        var dayExpenses = await _dayExpensesRepository.GetById(id, userName);

        var dto = new DayExpensesResponseDto
        {
            Id = dayExpenses.Id,
            Date = dayExpenses.Date,
            Participants = dayExpenses.Participants,
            Location = dayExpenses.Location
        };

        dto.TotalSum = await GetTotalSumForDayExpensesChecks(dto.Id);

        return dto;
    }

    public async Task<Guid> AddDayExpenses(CreateDayExpensesRequestDto dayExpensesRequestDto, string userName)
    {
        // Filter out empty participant names
        dayExpensesRequestDto.Participants = dayExpensesRequestDto.Participants
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .ToList();

        if (dayExpensesRequestDto.Participants.Count == 0)
            throw new ArgumentException("At least one participant is required.");

        var dayExpenses = dayExpensesRequestDto.ToDayExpenses(userName);
        return await _dayExpensesRepository.Insert(dayExpenses);
    }

    public async Task EditDayExpenses(EditDayExpensesRequestDto dayExpensesRequestDto, string userName)
    {
        // Filter out empty participant names
        dayExpensesRequestDto.Participants = dayExpensesRequestDto.Participants
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .ToList();

        if (dayExpensesRequestDto.Participants.Count == 0)
            throw new ArgumentException("At least one participant is required.");

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
        if (string.IsNullOrWhiteSpace(newUserWithAccess))
            return new ShareDayExpensesResponseDto(IsSuccess: false, Error: "Username is required.");

        var dayExpenses = await _dayExpensesRepository.GetById(id, userName);

        if (dayExpenses.PeopleWithAccess.Contains(newUserWithAccess))
            return new ShareDayExpensesResponseDto(IsSuccess: false, Error: $"{newUserWithAccess} already has access.");

        dayExpenses.PeopleWithAccess.Add(newUserWithAccess);
        await _dayExpensesRepository.Update(dayExpenses);

        return new ShareDayExpensesResponseDto(IsSuccess: true, Error: "");
    }

    public async Task<DayExpensesCalculationsDto> GetCalculationForDayExpenses(Guid id, string userName)
    {
        var dayExpenses = await GetById(id, userName);
        return await GetCalculations(dayExpenses);
    }

    public async Task<DayExpensesCalculationsDto> GetCalculations(DayExpensesResponseDto dayExpenses)
    {
        var dayExpensesCalculation = new DayExpensesCalculationsDto();

        if (dayExpenses is not null)
        {
            dayExpensesCalculation.DayExpensesId = dayExpenses.Id;
            dayExpensesCalculation.Participants = dayExpenses.Participants;
            dayExpensesCalculation.Checks = null;
            dayExpensesCalculation.DayExpensesCalculations = await CalculateDayExpensesList(dayExpenses);
            dayExpensesCalculation.AllUsersTrasactions = CalculateTransactionList(dayExpensesCalculation.DayExpensesCalculations);
            dayExpensesCalculation.OptimizedUserTransactions = OptimizeTransactions(dayExpensesCalculation.AllUsersTrasactions.ToList());
        }

        return dayExpensesCalculation;
    }

    private async Task<ICollection<DayExpensesCalculation>> CalculateDayExpensesList(DayExpensesResponseDto dayExpenses)
    {
        var dayExpensesCalculationList = new List<DayExpensesCalculation>();

        var dayExpensesChecks = await _checkService.GetAllDayExpensesChecks(dayExpenses.Id);
        foreach (var participant in dayExpenses.Participants)
        {
            var participantExpenses = new DayExpensesCalculation { UserName = participant, CheckCalculations = new List<CheckCalculation>() };
            foreach (var check in dayExpensesChecks)
            {
                var checkItems = await _itemRepository.GetAllCheckItems(check.Id);
                if (checkItems.Count > 0)
                {
                    var checkCalculation = new CheckCalculation { Check = check, Items = new List<ItemCalculation>() };
                    foreach (var item in checkItems)
                    {
                        if (item.Users.Contains(participant))
                        {
                            decimal pricePerUser = Math.Round(item.Price / item.Users.Count, 2);

                            checkCalculation.Items.Add(new ItemCalculation
                            {
                                Item = item,
                                PricePerUser = pricePerUser
                            });

                            checkCalculation.SumPerParticipant += pricePerUser;
                        }
                    }

                    if (checkCalculation.SumPerParticipant != 0)
                        participantExpenses.CheckCalculations.Add(checkCalculation);

                    participantExpenses.TotalSum += checkCalculation.SumPerParticipant;
                }
            }
            dayExpensesCalculationList.Add(participantExpenses);
        }

        return dayExpensesCalculationList;
    }

    private List<Transaction> CalculateTransactionList(ICollection<DayExpensesCalculation> dayExpensesCalculations)
    {
        List<Transaction> fullTransactionList = new List<Transaction>();
        foreach (var expensesCalculation in dayExpensesCalculations)
        {
            Dictionary<SenderRecipient, decimal> userTransactions = new Dictionary<SenderRecipient, decimal>();
            foreach (var checkCalculation in expensesCalculation.CheckCalculations)
            {
                if (checkCalculation.Check.Payer != expensesCalculation.UserName)
                {
                    var newTransaction = new Transaction
                    {
                        CheckName = checkCalculation.Check.Location,
                        Subjects = new SenderRecipient(expensesCalculation.UserName, checkCalculation.Check.Payer),
                        TransferAmount = checkCalculation.SumPerParticipant
                    };

                    fullTransactionList.Add(newTransaction);
                }
            }
        }

        return fullTransactionList;
    }

    private ICollection<Transaction> SumTransactions(List<Transaction> transactionList)
    {
        for (int i = 0; i < transactionList.Count; i++)
        {
            for (int j = 1; j < transactionList.Count; j++)
            {
                if (transactionList[i].Subjects.Equals(transactionList[j].Subjects) && i != j)
                {
                    transactionList[i].TransferAmount += transactionList[j].TransferAmount;
                    transactionList.Remove(transactionList[j]);
                }
            }
        }

        return transactionList;
    }

    private ICollection<Transaction> OptimizeTransactions(List<Transaction> transactionList)
    {
        transactionList = new List<Transaction>(transactionList.Select(t => (Transaction)t.Clone()));
        transactionList = SumTransactions(transactionList).ToList();
        for (int i = 0; i < transactionList.Count; i++)
        {
            for (int j = 1; j < transactionList.Count; j++)
            {
                if (transactionList[i].Subjects.Sender == transactionList[j].Subjects.Recipient &&
                    transactionList[i].Subjects.Recipient == transactionList[j].Subjects.Sender)
                {
                    if (transactionList[i].TransferAmount > transactionList[j].TransferAmount)
                    {
                        transactionList[i].TransferAmount -= transactionList[j].TransferAmount;
                        transactionList.Remove(transactionList[j]);
                    }
                    else if (transactionList[i].TransferAmount < transactionList[j].TransferAmount)
                    {
                        transactionList[j].TransferAmount -= transactionList[i].TransferAmount;
                        transactionList.Remove(transactionList[i]);
                    }
                    else
                    {
                        transactionList.Remove(transactionList[i]);
                        transactionList.Remove(transactionList[j]);
                    }
                }
            }
        }

        return transactionList;
    }

    private async Task<decimal> GetTotalSumForDayExpensesChecks(Guid dayExpensesId)
    {
        var checks = await _checkService.GetAllDayExpensesChecks(dayExpensesId);
        var totalSum = 0m;

        foreach (var check in checks)
        {
            var items = await _itemRepository.GetAllCheckItems(check.Id);
            totalSum += items.Sum(item => item.Price * item.Amount);
        }
        return totalSum;
    }
}
