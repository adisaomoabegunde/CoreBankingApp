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
    public class PendingRegistrationRepository : IPendingRegistrationRepository
    {
        private readonly AppDbContext _context;

        public PendingRegistrationRepository(AppDbContext context )
        {
            _context = context;
        }
        public async Task AddAsync(PendingRegistration pendingUser)
        {
            await _context.PendingRegistrations.AddAsync(pendingUser);
            await _context.SaveChangesAsync();
        }
        public async Task<PendingRegistration?> GetByEmailAsync(string email)
        {
            return await _context.PendingRegistrations
                .FirstOrDefaultAsync(x => x.Email == email);
        }
        public async Task DeleteAsync(PendingRegistration pendingUser)
        {
            _context.PendingRegistrations.Remove(pendingUser);
            await _context.SaveChangesAsync();
        }
    }
}
