namespace ExpensesCalculator.Models
{
    public class Transaction
    {
        public string CheckName { get; set; }
        public SenderRecipient Subjects { get; set; }   
        public decimal TransferAmount { get; set; }
    }
}
