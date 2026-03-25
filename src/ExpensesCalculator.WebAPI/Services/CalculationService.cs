using ExpensesCalculator.WebAPI.Models;
using ExpensesCalculator.WebAPI.Models.Dtos;
using ExpensesCalculator.WebAPI.Repositories.Interfaces;
using ExpensesCalculator.WebAPI.Services.Interfaces;

namespace ExpensesCalculator.WebAPI.Services;

public class ExpensesCalculatorService : IExpensesCalculator
{
    private readonly ICheckRepository _checkRepository;
    private readonly IItemRepository _itemRepository;

    public ExpensesCalculatorService(ICheckRepository checkRepository, IItemRepository itemRepository)
    {
        _checkRepository = checkRepository;
        _itemRepository = itemRepository;
    }

    public async Task<DayExpensesCalculationsDto> GetCalculations(DayExpensesResponseDto dayExpenses)
    {
        if (dayExpenses is null)
            return new DayExpensesCalculationsDto();

        var dayExpensesCalculations = await CalculateDayExpensesList(dayExpenses);
        var allTransactions = CalculateTransactionList(dayExpensesCalculations);
        var optimizedTransactions = OptimizeTransactions(allTransactions.ToList());

        return new DayExpensesCalculationsDto
        {
            DayExpensesId = dayExpenses.Id,
            Participants = dayExpenses.Participants,
            DayExpensesCalculations = dayExpensesCalculations,
            AllUsersTransactions = allTransactions,
            OptimizedUserTransactions = optimizedTransactions
        };
    }

    private async Task<ICollection<DayExpensesCalculation>> CalculateDayExpensesList(DayExpensesResponseDto dayExpenses)
    {
        var dayExpensesCalculationList = new List<DayExpensesCalculation>();

        var checks = await _checkRepository.GetAllDayChecks(dayExpenses.Id);

        // Load all items for all checks upfront to avoid N+1 query problem
        var checkItemsDict = new Dictionary<Guid, ICollection<Item>>();
        foreach (var check in checks)
            checkItemsDict[check.Id] = await _itemRepository.GetAllCheckItems(check.Id);

        foreach (var participant in dayExpenses.Participants)
        {
            var participantExpenses = new DayExpensesCalculation
            {
                UserName = participant,
                CheckCalculations = new List<CheckCalculation>()
            };

            foreach (var check in checks)
            {
                var items = checkItemsDict[check.Id];

                if (items.Count > 0)
                {
                    // Calculate total sum for this check
                    decimal checkTotalSum = items.Sum(item => item.Price * item.Amount);

                    var checkCalculation = new CheckCalculation
                    {
                        Check = new CheckCalculationData(check.Location, check.Payer, checkTotalSum),
                        Items = new List<ItemCalculation>()
                    };

                    foreach (var item in items)
                    {
                        if (item.Users.Contains(participant))
                        {
                            decimal pricePerUser = Math.Round(item.Price * item.Amount / item.Users.Count, 2);

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

                    participantExpenses.TotalSum += checkCalculation.SumPerParticipant;
                }
            }
            dayExpensesCalculationList.Add(participantExpenses);
        }

        return dayExpensesCalculationList;
    }

    private List<Transaction> CalculateTransactionList(ICollection<DayExpensesCalculation> dayExpensesCalculations)
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

    private ICollection<Transaction> SumTransactions(List<Transaction> transactionList)
    {
        for (int i = 0; i < transactionList.Count; i++)
        {
            for (int j = i + 1; j < transactionList.Count; j++)
            {
                if (transactionList[i].Subjects.Equals(transactionList[j].Subjects))
                {
                    transactionList[i].TransferAmount += transactionList[j].TransferAmount;
                    transactionList.RemoveAt(j);
                    j--;
                }
            }
        }

        return transactionList;
    }

    private ICollection<Transaction> OptimizeTransactions(List<Transaction> transactionList)
    {
        transactionList = new List<Transaction>(transactionList.Select(t => (Transaction)t.Clone()));
        transactionList = SumTransactions(transactionList).ToList();

        var result = new List<Transaction>();

        while (transactionList.Count > 0)
        {
            var current = transactionList[0];
            transactionList.RemoveAt(0);

            // Find opposite transaction (B→A when current is A→B)
            var oppositeIndex = transactionList.FindIndex(t =>
                t.Subjects.Sender == current.Subjects.Recipient &&
                t.Subjects.Recipient == current.Subjects.Sender);

            if (oppositeIndex >= 0)
            {
                var opposite = transactionList[oppositeIndex];
                transactionList.RemoveAt(oppositeIndex);

                // Net them out
                if (current.TransferAmount > opposite.TransferAmount)
                {
                    current.TransferAmount -= opposite.TransferAmount;
                    result.Add(current);
                }
                else if (current.TransferAmount < opposite.TransferAmount)
                {
                    opposite.TransferAmount -= current.TransferAmount;
                    result.Add(opposite);
                }
                // If equal, both cancel out - don't add either
            }
            else
            {
                // No opposite found, keep the transaction as-is
                result.Add(current);
            }
        }

        return result;
    }
}
