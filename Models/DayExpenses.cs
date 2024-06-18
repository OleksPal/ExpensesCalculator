namespace ExpensesCalculator.Models
{
    public class DayExpenses : DbObject
    {
        public List<Check> Checks { get; set; }
        public DateOnly Date { get; set; }
        public List<string> Participants { get; set; }
        public List<string> PeopleWithAccess { get; set; }
    }
}
