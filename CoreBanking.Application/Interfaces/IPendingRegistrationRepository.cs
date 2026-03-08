using CoreBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Interfaces
{
    public interface IPendingRegistrationRepository
    {
        Task AddAsync(PendingRegistration pendingUser);
        Task<PendingRegistration?> GetByEmailAsync(string email);
        Task DeleteAsync(PendingRegistration pendingUser);

    }
}
