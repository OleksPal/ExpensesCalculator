using ExpensesCalculator.Dtos.DayExpenses;
using ExpensesCalculator.Models;

namespace ExpensesCalculator.Mappers
{
    public static class DayExpensesMappers
    {
        public static DayExpenses ToDayExpenses(this CreateDayExpensesRequestDto dto, string creatorsUsername)
        {
            return new DayExpenses
            {
                Checks = new List<Check>(),
                Date = dto.Date,
                ParticipantsList = dto.ParticipantList,
                PeopleWithAccessList = new List<string>
                {
                    creatorsUsername
                }
            };
        }
    }
}
