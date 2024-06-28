using ExpensesCalculator.Models;

namespace ExpensesCalculator.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ExpensesContext context)
        {
            if (context.Days.Any() && context.Checks.Any() && context.Items.Any())
                return;

            var item = new Item { Id = 1, Name = "Item1", Description = "Description1", Price = 1000, CheckId = 1 };
            item.Users.Add("User1");
            item.Users.Add("User2");

            var check = new Check { Id = 1, Location = "Shop1", Sum = 1000, Payer = "User1", DayExpensesId = 1 };
            check.Items.Add(item);

            var dayExpenses = new DayExpenses { Id = 1, Date = new DateOnly(2024, 1, 1) };
            dayExpenses.Checks.Add(check);
            dayExpenses.Participants.Add("User1");
            dayExpenses.Participants.Add("User2");
            dayExpenses.PeopleWithAccess.Add("Guest");

            context.Items.Add(item);
            context.Checks.Add(check);
            context.Days.Add(dayExpenses);
            context.SaveChanges();
        }
    }
}
