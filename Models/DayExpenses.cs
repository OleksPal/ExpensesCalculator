namespace ExpensesCalculator.Models
{
    public class DayExpenses : DbObject
    {
        public ICollection<Check> Checks { get; } = new List<Check>();
        public DateOnly Date { get; set; }
        public ICollection<string> Participants { get; } = new List<string>();
        public ICollection<string> PeopleWithAccess { get; } = new List<string>();
    }
}
