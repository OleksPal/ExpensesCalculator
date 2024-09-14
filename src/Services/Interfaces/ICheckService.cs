using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Services
{
    public interface ICheckService
    {
        Task<Check> GetCheckById(Guid id);
        Task<Check> GetCheckByIdWithItems(Guid id);
        Task<SelectList> GetAllAvailableCheckPayers(Guid dayExpensesId);
        Task<Check> SetDayExpenses(Check check);
        Task<DayExpenses> AddCheck(Check check);
        Task<DayExpenses> EditCheck(Check check);
        Task<DayExpenses> DeleteCheck(Guid id);
    }
}
