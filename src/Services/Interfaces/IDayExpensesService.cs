﻿using ExpensesCalculator.Models;
using ExpensesCalculator.ViewModels;

namespace ExpensesCalculator.Services
{
    public interface IDayExpensesService
    {
        public string RequestorName { get; set; }

        Task<ICollection<DayExpensesViewModel>> GetAllDays();
        Task<DayExpenses> GetDayExpensesById(int id);
        Task<DayExpensesViewModel> GetDayExpensesViewModelById(int id);
        Task<DayExpenses> GetDayExpensesByIdWithChecks(int id);
        Task<DayExpenses> GetFullDayExpensesById(int id);
        Task<DayExpenses> AddDayExpenses(DayExpenses dayExpenses);
        Task<DayExpenses> EditDayExpenses(DayExpenses dayExpenses);
        Task<DayExpenses> DeleteDayExpenses(int dayExpensesId);
        Task<DayExpensesCalculationViewModel> GetCalculationForDayExpenses(int id);
        Task<string> GetFormatParticipantsNames(IEnumerable<string> participantList);        
        Task<string?> ChangeDayExpensesAccess(int id, string newUserWithAccess);
    }
}
