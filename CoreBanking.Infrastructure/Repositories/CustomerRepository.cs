using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Customers.AnyAsync(x => x.Email == email);
        }
        public async Task<bool> PhoneExistsAsync(string phoneNumber)
        {
            return await _context.Customers.AnyAsync(x => x.PhoneNumber == phoneNumber);
        }
        public async Task<bool> BvnExistsAsync(string bvn)
        {
            return await _context.Customers.AnyAsync(x => x.BVN == bvn);
        }
        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }
        public async Task<Customer> GetByIdAsync(Guid id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
        public async Task<Customer> GetByUserIdAsync(Guid userId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
