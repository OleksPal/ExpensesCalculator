using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpensesCalculator.Services.Interfaces;

public interface ICheckService
{
    Task<Check> GetById(int id);
    Task<SelectList> GetAllAvailableCheckPayers(int dayExpensesId);
    Task AddCheck(Check check);
    Task EditCheck(Check check);
    Task DeleteCheck(int id);
}
