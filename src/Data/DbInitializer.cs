using ExpensesCalculator.Models;

namespace ExpensesCalculator.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ExpensesContext context)
        {
            if (context.Days.Any() && context.Checks.Any() && context.Items.Any()
                && context.Users.Any())
                return;

            var dayExpenses = new DayExpenses
            {
                Date = new DateOnly(2024, 1, 1),
                ParticipantsList = ["User1", "User2"],
                PeopleWithAccessList = ["Guest"]
            };

            context.Days.Add(dayExpenses);
            context.SaveChanges();

            int dayExpensesId = context.Days.First().Id;
            var check = new Check
            {
                Location = "Shop1",
                Sum = 1000,
                Payer = "User1",
                DayExpensesId = dayExpensesId
            };

            context.Checks.Add(check);
            context.SaveChanges();

            int checkId = context.Checks.First().Id;
            var item = new Item
            {
                Name = "Item1",
                Description = "Description1",
                Price = 1000,
                CheckId = checkId,
                UsersList = ["User1", "User2"]
            };

            context.Items.Add(item);
            context.SaveChanges();

            var user = new User();

            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
