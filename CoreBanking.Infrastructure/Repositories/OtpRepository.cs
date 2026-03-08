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
    public class OtpRepository : IOtpRepository
    {
        private readonly AppDbContext _context;

        public OtpRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Otp otp)
        {
            await _context.Otps.AddAsync(otp);
            await _context.SaveChangesAsync();
        }
        public async Task<Otp?> GetValidOtp(Guid userId, string code)
        {
            return await _context.Otps
                .FirstOrDefaultAsync(o =>
                o.UserId == userId &&
                o.Code == code &&
                !o.IsUsed);
        }
        public async Task UpdateAsync(Otp otp)
        {
            _context.Otps.Update(otp);
            await _context.SaveChangesAsync();
        }

    }
}
