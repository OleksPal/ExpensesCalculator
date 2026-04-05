namespace ExpensesCalculator.WebAPI.Models;

public class DayExpensesCalculation
{
    public string UserName { get; set; }
    public ICollection<CheckCalculation> CheckCalculations { get; set; }
    public decimal TotalSum { get; set; }
}

public class CheckCalculation
{
    public CheckCalculationData Check { get; set; }
    public ICollection<ItemCalculation> Items { get; set; }
    public decimal SumPerParticipant { get; set; }
}

public class ItemCalculation
{
    public Item Item { get; set; }
    public decimal PricePerUser { get; set; }
}

public class Transaction : ICloneable
{
    public string CheckName { get; set; }
    public SenderRecipient Subjects { get; set; }
    public decimal TransferAmount { get; set; }
    public object Clone() => MemberwiseClone();
}

public record SenderRecipient(string Sender, string Recipient);

public record CheckCalculationData(string Location, string Payer, decimal TotalSum);
