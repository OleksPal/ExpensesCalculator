using ExpensesCalculator.Models;

namespace ExpensesCalculator.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ExpensesContext context)
        {
            if (context.Days.Any() && context.Checks.Any() && context.Items.Any())
                return;

            var dayExpenses = new DayExpenses
            {
                Date = new DateOnly(2025, 1, 1),
                ParticipantsList = ["User1", "User2"],
                PeopleWithAccessList = ["Guest"]
            };

            context.Days.Add(dayExpenses);
            context.SaveChanges();

            var check = new Check
            {
                Location = "Shop1",
                Sum = 1000,
                Payer = "User1",
                DayExpensesId = 1
            };

            context.Checks.Add(check);
            context.SaveChanges();

            var tag = new Tag
            {
                Name = "Meat",
                Color = "#FF0000"
            };

            context.Tags.Add(tag);
            context.SaveChanges();

            var item = new Item
            {
                Name = "Item1",
                Description = "Description1",
                Price = 1000,
                CheckId = 1,
                UserList = ["User1", "User2"]
            };

            context.Items.Add(item);
            context.SaveChanges();

            var itemTag = new ItemTag
            {
                ItemId = 1,
                TagId = 1
            };
            context.ItemTags.Add(itemTag);
            context.SaveChanges();
        }
    }
}
