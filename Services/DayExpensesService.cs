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
                dayExpensesCalculation.AllUsersTrasactions = CalculateTransactionList(dayExpensesCalculation.Checks);
                dayExpensesCalculation.OptimizedUserTransactions = OptimizeTransactions(dayExpensesCalculation.AllUsersTrasactions);
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

        private List<Transaction> CalculateTransactionList(IEnumerable<Check> checks)
        {
            List<Transaction> fullTransactionList = new();
            foreach (var check in checks)
            {
                Dictionary<SenderRecipient, decimal> userTransactions = new();
                foreach (var item in check.Items)
                {
                    foreach (var user in item.UsersList)
                    {
                        if (user != check.Payer)
                        {
                            var transactionKey = new SenderRecipient(user, check.Payer);
                            decimal transactionValue = (decimal)(item.Price / item.UsersList.Count);

                            if (!userTransactions.Keys.Any(key => key.Equals(transactionKey)))
                                userTransactions.Add(transactionKey, transactionValue);
                            else
                                userTransactions[transactionKey] += transactionValue;
                        }
                    }
                }

                foreach (var transaction in userTransactions)
                {
                    var newTransaction = new Transaction();
                    newTransaction.CheckName = check.Location;
                    newTransaction.Subjects = transaction.Key;
                    newTransaction.TransferAmount = transaction.Value;
                    fullTransactionList.Add(newTransaction);
                }
            }

            return fullTransactionList;
        }

        private ICollection<Transaction> OptimizeTransactions(ICollection<Transaction> transactionList)
        {
            foreach(var transaction in transactionList)
            {
                foreach(var nextTransaction in transactionList)
                {
                    if (transaction.Subjects.Sender == nextTransaction.Subjects.Recipient &&
                        transaction.Subjects.Recipient == nextTransaction.Subjects.Sender) 
                    {
                        if (transaction.TransferAmount > nextTransaction.TransferAmount)
                        {
                            transaction.TransferAmount -= nextTransaction.TransferAmount;
                            transactionList.Remove(nextTransaction);
                        }
                        else if (transaction.TransferAmount < nextTransaction.TransferAmount)
                        {
                            nextTransaction.TransferAmount -= transaction.TransferAmount;
                            transactionList.Remove(transaction);
                        }
                        else
                        {
                            transactionList.Remove(transaction);
                            transactionList.Remove(nextTransaction);
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
