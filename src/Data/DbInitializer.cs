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
            
            var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "Test",
                    Email = "test@gmail.com",
                    PasswordHash = String.Empty,
                };

            context.Users.Add(user);

            var dayExpenses = new DayExpenses
            {
                Id = Guid.NewGuid(),
                Date = new DateOnly(2024, 1, 1),
                ParticipantsList = ["User1", "User2"],
                PeopleWithAccessList = ["Guest"]
            };

            context.Days.Add(dayExpenses);

            var check = new Check
            {
                Id = Guid.NewGuid(),
                Location = "Shop1",
                Sum = 1000,
                Payer = "User1",
                DayExpensesId = dayExpenses.Id
            };

            context.Checks.Add(check);

            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = "Item1",
                Description = "Description1",
                Price = 1000,
                CheckId = check.Id,
                UsersList = ["User1", "User2"]
            };

            context.Items.Add(item);
            context.SaveChanges();
        }
    }
}
