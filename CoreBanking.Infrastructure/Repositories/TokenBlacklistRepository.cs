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
    public class TokenBlacklistRepository : ITokenBlacklistRepository
    {
        private readonly AppDbContext _context;

        public TokenBlacklistRepository(AppDbContext context) {

            _context = context;
        }
        public async Task AddAsync(RevokedToken token)
        {
            await _context.RevokedTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> IsTokenRevokedAsync(string token)
        {
            return await _context.RevokedTokens
                .AnyAsync(x => x.Token == token);
        }
    }
}
