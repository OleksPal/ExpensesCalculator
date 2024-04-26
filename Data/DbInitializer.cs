using ExpensesCalculator.Models;

namespace ExpensesCalculator.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ExpensesContext context)
        {
            if (context.Days.Any() && context.Checks.Any() && context.Items.Any())
                return;

            var cognac = new Item { Name = "Erisioni", Description = "0,5L", Price = 172.7 };
            var check = new Check { Items = [cognac], Location = "ATB", Sum = 172.7 };
            var dayExpenses = new DayExpenses { Checks = [check], Date = new DateOnly(2024, 1, 1) };

            context.Items.Add(cognac);
            context.Checks.Add(check);
            context.Days.Add(dayExpenses);
            context.SaveChanges();
        }
    }
}
