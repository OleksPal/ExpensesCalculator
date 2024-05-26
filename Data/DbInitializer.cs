using ExpensesCalculator.Models;

namespace ExpensesCalculator.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ExpensesContext context)
        {
            if (context.Days.Any() && context.Checks.Any() && context.Items.Any())
                return;

            var item = new Item { Name = "Item1", Description = String.Empty, Price = 1000, Users = ["User1", "User2"] };
            var check = new Check { Items = [item], Location = "Shop1", Sum = 1000, Payer = "User1" };
            var dayExpenses = new DayExpenses { Checks = [check], Date = new DateOnly(2024, 1, 1), Participants = ["User1", "User2"] };

            context.Items.Add(item);
            context.Checks.Add(check);
            context.Days.Add(dayExpenses);
            context.SaveChanges();
        }
    }
}
