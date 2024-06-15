namespace ExpensesCalculator.Models
{
    public struct SenderRecipient
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }

        public SenderRecipient(string sender, string recipient)
        {
            Sender = sender;
            Recipient = recipient;
        }
    }
}
