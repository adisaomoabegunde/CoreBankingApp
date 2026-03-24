using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Interfaces
{
    public interface IAccountRepository
    {
        Task AddAsync(Account account);
        Task<Account?> GetByCustomerAndType(Guid customerId, AccountType type);
        Task<bool> AccountNumberExists(string accountNumber);
        Task<Account?> GetbyAccountNumberAsync(string accountNumber);
        Task<List<Account>> GetAllAsync();
        Task<Account?> GetByAccountNumberAsync(string accountNumber);
        Task<List<Account>> GetByCustomerIdAsync(Guid customerId);
        Task UpdateAsync(Account account);

    }
}
