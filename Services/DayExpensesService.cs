using ExpensesCalculator.Models;
using ExpensesCalculator.Repositories;
using NuGet.Packaging;
using System.Text.RegularExpressions;

namespace ExpensesCalculator.Services
{
    public class DayExpensesService : IDayExpensesService
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICheckRepository _checkRepository;
        private readonly IDayExpensesRepository _dayExpensesRepository;
        private readonly IUserRepository _userRepository;

        public string RequestorName { get; set; } = "Guest";

        public DayExpensesService(IItemRepository itemRepository, 
            ICheckRepository checkRepository, IDayExpensesRepository dayExpensesRepository, 
            IUserRepository userRepository)
        {
            _itemRepository = itemRepository;
            _checkRepository = checkRepository;
            _dayExpensesRepository = dayExpensesRepository;
            _userRepository = userRepository;
        }

        public async Task<ICollection<DayExpenses>> GetAllDays()
        {            
            var result = await _dayExpensesRepository.GetAll();

            return result.Where(r => r.PeopleWithAccessList.Contains(RequestorName)).ToList();
        }

        public async Task<DayExpenses> GetDayExpensesById(int id)
        {
            var result = await _dayExpensesRepository.GetById(id);

            return result.PeopleWithAccess.Contains(RequestorName) ? result : null;
        }

        public async Task<DayExpenses> GetDayExpensesByIdWithChecks(int id)
        {
            var dayExpenses = await GetDayExpensesById(id);

            if (dayExpenses is not null)
            {
                var checks = await _checkRepository.GetAllDayChecks(id);
                dayExpenses.Checks = checks.ToList();
            }                

            return dayExpenses;
        }

        public async Task<DayExpenses> GetFullDayExpensesById(int id)
        {
            var dayExpenses = await GetDayExpensesByIdWithChecks(id);

            if (dayExpenses is not null)
            {
                foreach (var check in dayExpenses.Checks)
                {
                    var items = await _itemRepository.GetAllCheckItems(check.Id);
                    check.Items = items.ToList();
                }   
            }

            return dayExpenses;
        }

        public async Task<DayExpenses> AddDayExpenses(DayExpenses dayExpenses)
        {
            string rareNameList = dayExpenses.ParticipantsList.First();
            dayExpenses.ParticipantsList.Clear();
            dayExpenses.ParticipantsList.AddRange(GetParticipantListFromString(rareNameList));

            return await _dayExpensesRepository.Insert(dayExpenses);
        }

        public async Task<DayExpenses> EditDayExpenses(DayExpenses dayExpenses)
        {
            if (dayExpenses.PeopleWithAccessList.Contains(RequestorName))
            {
                string rareNameList = dayExpenses.ParticipantsList.First();
                dayExpenses.ParticipantsList.Clear();
                dayExpenses.ParticipantsList.AddRange(GetParticipantListFromString(rareNameList));

                dayExpenses = await _dayExpensesRepository.Update(dayExpenses);
            }

            return dayExpenses;
        }

        public async Task<DayExpenses> DeleteDayExpenses(int id)
        {
            var dayExpensesToDelete = await GetDayExpensesById(id);

            if (dayExpensesToDelete is not null)
                dayExpensesToDelete = await _dayExpensesRepository.Delete(id);

            return dayExpensesToDelete;
        }

        public async Task<DayExpensesCalculationViewModel> GetCalculationForDayExpenses(int id)
        {
            var dayExpenses = await GetFullDayExpensesById(id);
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

        public async Task<string> GetFormatParticipantsNames(IEnumerable<string> participantList)
        {
            return String.Join(", ", participantList);
        }

        public async Task<string?> ChangeDayExpensesAccess(int id, string newUserWithAccess)
        {
            var dayExpenses = await GetDayExpensesById(id);

            if (dayExpenses is not null)
            {
                bool isUserExist = _userRepository.UserExists(newUserWithAccess);

                if (!isUserExist)
                    return "There is no such user!";
                else if (dayExpenses.PeopleWithAccess.Contains(newUserWithAccess))
                    return "This user already has access!";
                else
                {
                    dayExpenses.PeopleWithAccessList.Add(newUserWithAccess);
                    await _dayExpensesRepository.Update(dayExpenses);
                    return "Done!";
                }
            }

            return null;
        }

        private ICollection<DayExpensesCalculation> CalculateDayExpensesList(DayExpenses dayExpenses)
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

                        if(checkCalculation.SumPerParticipant != 0)
                            participantExpenses.CheckCalculations.Add(checkCalculation);
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

        private ICollection<Transaction> OptimizeTransactions(List<Transaction> transactionList)
        {
            transactionList = new List<Transaction>(transactionList.Select(t => (Transaction)t.Clone()));
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

        private IEnumerable<string> GetParticipantListFromString(string rareText)
        {
            Regex pattern = new Regex(@"\w+");

            var matchList = pattern.Matches(rareText).ToList();
            List<string> participantList = new List<string>();

            foreach (var match in matchList)
                participantList.Add(match.Value);

            return participantList;
        }        
    }
}
