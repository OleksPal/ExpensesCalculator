using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Services
{
    public interface ICheckService
    {
        Task<Check> GetCheckById(int id);
        Task<Check> GetCheckByIdWithItems(int id);
        Task<bool> CheckExists(int id);
        Task<SelectList> GetAllAvailableCheckPayers(int dayExpensesId);
        Task<DayExpenses> AddCheck(Check check, int dayExpensesId);
        Task<DayExpenses> EditCheck(Check check, int dayExpensesId);
        Task<DayExpenses> DeleteCheck(int id, int dayExpensesId);
    }
}
