﻿using ExpensesCalculator.Models;
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
            return result.Where(r => r.PeopleWithAccess.Contains(RequestorName)).ToList();
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
                dayExpenses.Checks.AddRange(await _checkRepository.GetAllDayChecks(id));

            return dayExpenses;
        }

        public async Task<DayExpenses> GetFullDayExpensesById(int id)
        {
            var dayExpenses = await GetDayExpensesById(id);

            if (dayExpenses is not null)
            {
                dayExpenses.Checks.AddRange(await _checkRepository.GetAllDayChecks(id));

                foreach (var check in dayExpenses.Checks)
                    check.Items.AddRange(await _itemRepository.GetAllCheckItems(check.Id));
            }

            return dayExpenses;
        }

        public async Task<bool> DayExpensesExists(int id)
        {
            return await GetDayExpensesById(id) is not null;
        }

        public async Task<DayExpensesCalculationViewModel> GetCalculationForDayExpenses(int id)
        {
            var dayExpenses = await GetFullDayExpensesById(id);
            var dayExpensesCalculation = new DayExpensesCalculationViewModel();

            if (dayExpenses is not null)
            {
                dayExpensesCalculation.DayExpensesId = dayExpenses.Id;
                dayExpensesCalculation.Participants = dayExpenses.Participants.ToList();
                dayExpensesCalculation.Checks = dayExpenses.Checks.ToList();
                dayExpensesCalculation.AllUsersTrasactions = CalculateTransactionList(dayExpensesCalculation.Checks);

                var transactionsDictionary = new Dictionary<SenderRecipient, decimal>();
                foreach (var transaction in dayExpensesCalculation.AllUsersTrasactions)
                {
                    if (!transactionsDictionary.Keys.Any(key => key.Equals(transaction.Subjects)))
                        transactionsDictionary.Add(transaction.Subjects, transaction.TransferAmount);
                    else
                        transactionsDictionary[transaction.Subjects] += transaction.TransferAmount;
                }

                var optimizedTransactions = OptimizeTransactions(transactionsDictionary);
                dayExpensesCalculation.OptimizedUserTransactions = optimizedTransactions;
            }

            return dayExpensesCalculation;
        }

        public async Task<DayExpenses> AddDayExpenses(DayExpenses dayExpenses)
        {
            string rareNameList = dayExpenses.Participants.ToList()[0];
            dayExpenses.Participants.Clear();
            dayExpenses.Participants.AddRange(GetParticipantListFromString(rareNameList));

            await _dayExpensesRepository.Insert(dayExpenses);

            return dayExpenses;
        }

        public async Task<DayExpenses> EditDayExpenses(DayExpenses dayExpenses)
        {
            string rareNameList = dayExpenses.Participants.ToList()[0];
            dayExpenses.Participants.Clear();
            dayExpenses.Participants.AddRange(GetParticipantListFromString(rareNameList));

            await _dayExpensesRepository.Update(dayExpenses);

            return dayExpenses;
        }

        public async Task<DayExpenses> DeleteDayExpenses(int id)
        {
            var dayExpensesToDelete = await GetDayExpensesById(id);

            await _dayExpensesRepository.Delete(id);

            return dayExpensesToDelete;
        }

        public async Task<string> GetFormatParticipantsNames(int id)
        {
            var dayExpenses = await GetDayExpensesById(id);
            var participants = dayExpenses.Participants.ToList();

            string formatList = String.Empty;
            for (int i = 0; i < participants.Count; i++)
            {
                if (participants[i] is not null)
                {
                    formatList += participants[i];
                    if (i != participants.Count - 1)
                        formatList += ", ";
                }
            }

            return formatList;
        }

        private List<Transaction> CalculateTransactionList(List<Check> checks)
        {
            List<Transaction> fullTransactionList = new();
            foreach (var check in checks)
            {
                Dictionary<SenderRecipient, decimal> userTransactions = new();
                foreach (var item in check.Items)
                {
                    foreach (var user in item.Users)
                    {
                        if (user != check.Payer)
                        {
                            var transactionKey = new SenderRecipient(user, check.Payer);
                            decimal transactionValue = (decimal)(item.Price / item.Users.Count);

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

        private Dictionary<SenderRecipient, decimal> OptimizeTransactions(Dictionary<SenderRecipient, decimal> transactionList)
        {
            var transactionKeyList = transactionList.Keys.ToList();
            for (int i = 0; i < transactionKeyList.Count; i++)
            {
                for (int j = 1; j < transactionKeyList.Count; j++)
                {
                    if (transactionKeyList[i].Sender == transactionKeyList[j].Recipient &&
                        transactionKeyList[i].Recipient == transactionKeyList[j].Sender)
                    {
                        if (transactionList[transactionKeyList[i]] > transactionList[transactionKeyList[j]])
                        {
                            transactionList[transactionKeyList[i]] -= transactionList[transactionKeyList[j]];
                            transactionList.Remove(transactionKeyList[j]);
                        }
                        else if (transactionList[transactionKeyList[i]] < transactionList[transactionKeyList[j]])
                        {
                            transactionList[transactionKeyList[j]] -= transactionList[transactionKeyList[i]];
                            transactionList.Remove(transactionKeyList[i]);
                        }
                        else
                        {
                            transactionList.Remove(transactionKeyList[i]);
                            transactionList.Remove(transactionKeyList[j]);
                        }
                    }
                }
            }

            return transactionList;
        }

        private IEnumerable<string> GetParticipantListFromString(string rareText)
        {
            Regex spacesAfterComma = new Regex(@",\s+"),
                bigSpaces = new Regex(@"\s+");
            rareText = bigSpaces.Replace(rareText, " ");
            rareText = spacesAfterComma.Replace(rareText, ",");

            List<string> participantList = rareText.Split(',').ToList();
            participantList = participantList.Select(p => p != null ? p.Trim() : null).ToList();

            return participantList;
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
                    dayExpenses.PeopleWithAccess.Add(newUserWithAccess);
                    return "Done!";
                }
            }

            return null;
        }
    }
}