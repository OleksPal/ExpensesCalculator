using ExpensesCalculator.Models;

namespace ExpensesCalculator.Extensions
{
    public static class DayExpensesExtensions
    {
        public static DayExpensesCalculationViewModel GetCalculations(this DayExpenses dayExpenses)
        {
            var dayExpensesCalculation = new DayExpensesCalculationViewModel();

            if (dayExpenses is not null)
            {
                dayExpensesCalculation.DayExpensesId = dayExpenses.Id;
                dayExpensesCalculation.Participants = dayExpenses.ParticipantsList;
                dayExpensesCalculation.Checks = dayExpenses.Checks;
                dayExpensesCalculation.DayExpensesCalculations = CalculateDayExpensesList(dayExpenses);
                dayExpensesCalculation.AllUsersTrasactions = CalculateTransactionList(dayExpensesCalculation.DayExpensesCalculations);
                dayExpensesCalculation.OptimizedUserTransactions = OptimizeTransactions(dayExpensesCalculation.AllUsersTrasactions.ToList());
            }

            return dayExpensesCalculation;
        }

        private static ICollection<DayExpensesCalculation> CalculateDayExpensesList(DayExpenses dayExpenses)
        {
            var dayExpensesCalculationList = new List<DayExpensesCalculation>();
            foreach (var participant in dayExpenses.ParticipantsList)
            {
                var participantExpenses = new DayExpensesCalculation { UserName = participant };
                foreach (var check in dayExpenses.Checks)
                {
                    if (check.Items.Count > 0)
                    {
                        var checkCalculation = new CheckCalculation { Check = check };
                        foreach (var item in check.Items)
                        {
                            if (item.UsersList.Contains(participant))
                            {
                                decimal pricePerUser = Math.Round(item.Price / item.UsersList.Count, 2);

                                checkCalculation.Items.Add(new ItemCalculation
                                {
                                    Item = item,
                                    PricePerUser = pricePerUser
                                });

                                checkCalculation.SumPerParticipant += pricePerUser;
                            }
                        }

                        if (checkCalculation.SumPerParticipant != 0)
                            participantExpenses.CheckCalculations.Add(checkCalculation);
                    }
                }
                dayExpensesCalculationList.Add(participantExpenses);
            }

            return dayExpensesCalculationList;
        }

        private static List<Transaction> CalculateTransactionList(ICollection<DayExpensesCalculation> dayExpensesCalculations)
        {
            List<Transaction> fullTransactionList = new List<Transaction>();
            foreach (var expensesCalculation in dayExpensesCalculations)
            {
                Dictionary<SenderRecipient, decimal> userTransactions = new Dictionary<SenderRecipient, decimal>();
                foreach (var checkCalculation in expensesCalculation.CheckCalculations)
                {
                    if (checkCalculation.Check.Payer != expensesCalculation.UserName)
                    {
                        var newTransaction = new Transaction
                        {
                            CheckName = checkCalculation.Check.Location,
                            Subjects = new SenderRecipient(expensesCalculation.UserName, checkCalculation.Check.Payer),
                            TransferAmount = checkCalculation.SumPerParticipant
                        };

                        fullTransactionList.Add(newTransaction);
                    }
                }
            }

            return fullTransactionList;
        }

        private static ICollection<Transaction> SumTransactions(List<Transaction> transactionList)
        {
            for (int i = 0; i < transactionList.Count; i++)
            {
                for (int j = 1; j < transactionList.Count; j++)
                {
                    if (transactionList[i].Subjects.Equals(transactionList[j].Subjects) && i != j)
                    {
                        transactionList[i].TransferAmount += transactionList[j].TransferAmount;
                        transactionList.Remove(transactionList[j]);
                    }
                }
            }

            return transactionList;
        }

        private static ICollection<Transaction> OptimizeTransactions(List<Transaction> transactionList)
        {
            transactionList = new List<Transaction>(transactionList.Select(t => (Transaction)t.Clone()));
            transactionList = SumTransactions(transactionList).ToList();
            for (int i = 0; i < transactionList.Count; i++)
            {
                for (int j = 1; j < transactionList.Count; j++)
                {
                    if (transactionList[i].Subjects.Sender == transactionList[j].Subjects.Recipient &&
                        transactionList[i].Subjects.Recipient == transactionList[j].Subjects.Sender)
                    {
                        if (transactionList[i].TransferAmount > transactionList[j].TransferAmount)
                        {
                            transactionList[i].TransferAmount -= transactionList[j].TransferAmount;
                            transactionList.Remove(transactionList[j]);
                        }
                        else if (transactionList[i].TransferAmount < transactionList[j].TransferAmount)
                        {
                            transactionList[j].TransferAmount -= transactionList[i].TransferAmount;
                            transactionList.Remove(transactionList[i]);
                        }
                        else
                        {
                            transactionList.Remove(transactionList[i]);
                            transactionList.Remove(transactionList[j]);
                        }
                    }
                }
            }

            return transactionList;
        }
    }
}
