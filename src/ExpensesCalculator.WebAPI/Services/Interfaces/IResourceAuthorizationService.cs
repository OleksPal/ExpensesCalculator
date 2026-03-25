namespace ExpensesCalculator.WebAPI.Services.Interfaces;

public interface IResourceAuthorizationService
{
    Task ValidateUserAccessToCheck(Guid checkId, string userName);
    Task ValidateUserAccessToItem(Guid itemId, string userName);
    Task ValidateUserAccessToDayExpenses(Guid dayExpensesId, string userName);
}
