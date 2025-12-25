using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;

namespace ExpensesCalculator.WebAPI.Helpers;

public static class DayExpensesMapper
{
    public static DayExpenses ToDayExpenses(this CreateDayExpensesRequestDto dto, string userName)
    {
        return new DayExpenses
        {
            Date = dto.Date,
            Participants = dto.Participants,
            PeopleWithAccess = [userName],
            Location = dto.Location
        };
    }    
}

public static class DayExpensesHelper
{
    public static void UpdateDayExpenses(ref DayExpenses dayExpenses, EditDayExpensesRequestDto dto)
    {
        dayExpenses.Date = dto.Date;
        dayExpenses.Participants = dto.Participants;
        dayExpenses.Location = dto.Location;
    }
}
