using CoreBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phoneNumber);
        Task<bool> BvnExistsAsync(string bvn);
        Task<List<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(Guid id);
        Task UpdateAsync(Customer customer);
        Task<Customer> GetByUserIdAsync(Guid userId);
    }
}
