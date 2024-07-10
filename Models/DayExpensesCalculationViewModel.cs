﻿namespace ExpensesCalculator.Models
{
    public class DayExpensesCalculationViewModel
    {
        public int DayExpensesId { get; set; }
        public IEnumerable<string> Participants { get; set; }
        public IEnumerable<Check> Checks { get; set; }
        public ICollection<Transaction> AllUsersTrasactions { get; set; }
        public ICollection<Transaction> OptimizedUserTransactions { get; set; }
    }
}
