namespace ExpensesCalculator.Models
{
    public class Transaction : ICloneable
    {
        public string CheckName { get; set; }
        public SenderRecipient Subjects { get; set; }   
        public decimal TransferAmount { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
