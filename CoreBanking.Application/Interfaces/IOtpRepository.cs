using CoreBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Interfaces
{
    public interface IOtpRepository
    {
        Task AddAsync(Otp otp);
        Task<Otp?> GetValidOtp(Guid userId, string code);
        Task UpdateAsync(Otp otp);
    }
}
