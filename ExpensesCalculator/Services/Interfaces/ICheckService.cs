using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Services
{
    public interface ICheckService
    {
        Task<Check> GetCheckById(int id);
        Task<Check> GetCheckByIdWithItems(int id);
        Task<SelectList> GetAllAvailableCheckPayers(int dayExpensesId);
        Task<Check> SetDayExpenses(Check check);
        Task<DayExpenses> AddCheck(Check check);
        Task<DayExpenses> EditCheck(Check check);
        Task<DayExpenses> DeleteCheck(int id);
    }
}
