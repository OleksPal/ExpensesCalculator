using System.ComponentModel.DataAnnotations;

namespace ExpensesCalculator.Models
{
    public class Transaction : ICloneable
    {
        public string CheckName { get; set; }
        public SenderRecipient Subjects { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n2}₴")]
        public decimal TransferAmount { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
