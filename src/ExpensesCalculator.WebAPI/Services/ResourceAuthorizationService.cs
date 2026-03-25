using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using ExpensesCalculator.WebAPI.Services.Interfaces;

namespace ExpensesCalculator.WebAPI.Services;

public class ResourceAuthorizationService : IResourceAuthorizationService
{
    private readonly ICheckRepository _checkRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IDayExpensesRepository _dayExpensesRepository;

    public ResourceAuthorizationService(
        ICheckRepository checkRepository,
        IItemRepository itemRepository,
        IDayExpensesRepository dayExpensesRepository)
    {
        _checkRepository = checkRepository;
        _itemRepository = itemRepository;
        _dayExpensesRepository = dayExpensesRepository;
    }

    public async Task ValidateUserAccessToCheck(Guid checkId, string userName)
    {
        var check = await _checkRepository.GetById(checkId);
        if (check == null)
            throw new KeyNotFoundException($"Check {checkId} not found");

        await ValidateUserAccessToDayExpenses(check.DayExpensesId, userName);
    }

    public async Task ValidateUserAccessToItem(Guid itemId, string userName)
    {
        var item = await _itemRepository.GetById(itemId);
        if (item == null)
            throw new KeyNotFoundException($"Item {itemId} not found");

        await ValidateUserAccessToCheck(item.CheckId, userName);
    }

    public async Task ValidateUserAccessToDayExpenses(Guid dayExpensesId, string userName)
    {
        var dayExpenses = await _dayExpensesRepository.GetByIdInternal(dayExpensesId);

        if (dayExpenses == null)
            throw new KeyNotFoundException($"Day expenses {dayExpensesId} not found");

        if (!dayExpenses.PeopleWithAccess.Contains(userName))
            throw new UnauthorizedAccessException($"User {userName} doesn't have access to this resource");
    }
}
