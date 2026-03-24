using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Infrastructure.Services
{
    public class AccountNumberGenerator : IAccountNumberGenerator
    {
        private readonly IAccountRepository _accountRepository;
        public AccountNumberGenerator(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<string> GenerateAccountNumberAsync(AccountType type)
        {
            string prefix = type switch
            {
                AccountType.Savings => "001",
                AccountType.Current => "002",
                AccountType.FixedDeposit => "003",
                _ => throw new Exception("Invalid account type")

            };
            string accountNumber;

            do
            {
                var random = new Random();
                var suffix = random.Next(1000000, 9999999).ToString();

                accountNumber = prefix + suffix;

            } while (await _accountRepository.AccountNumberExists(accountNumber));

            return accountNumber;
        }
    }
}
