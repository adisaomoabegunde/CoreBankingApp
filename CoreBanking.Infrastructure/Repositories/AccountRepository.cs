using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;
using CoreBanking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;
        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> AccountNumberExists(string accountNumber)
        {
            return await _context.Accounts
                .AnyAsync(x => x.AccountNumber == accountNumber);
        }
        public async Task<Account?> GetByCustomerAndType(Guid customerId, AccountType type)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.AccountType == type);
        }
        public async Task<Account?> GetbyAccountNumberAsync(string accountNumber)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);
        }
        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .OrderByDescending(x => x.DateOpened)
                .ToListAsync();
        }
        public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);
        }
        public async Task<List<Account>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.Accounts
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.DateOpened)
                .ToListAsync();
        }
        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
        public async Task<Account?> GetByAccountNumberForUpdateAsync(string accountNumber)
        {
            return await _context.Accounts
                .Where(a => a.AccountNumber == accountNumber)
                .FirstOrDefaultAsync();
        }
        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> IsOwnedByCustomer(Guid accountId, Guid customerId)
        {
            return await _context.Accounts
                .AnyAsync(a => a.Id == accountId && a.CustomerId == customerId);
        }

        public async Task<(List<Account>, int)> GetAllWithPaginationAsync(
            int pageNumber,
            int pageSize)
                {
                    var query = _context.Accounts
                        .Include(a => a.Customer) // 👈 IMPORTANT for name
                        .AsQueryable();

                    var totalRecords = await query.CountAsync();

                    var data = await query
                        .OrderByDescending(a => a.DateOpened)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

                    return (data, totalRecords);
                }
    }
}
