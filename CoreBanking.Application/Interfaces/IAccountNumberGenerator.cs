using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Interfaces
{
    public interface IAccountNumberGenerator
    {
        Task<string> GenerateAccountNumberAsync(AccountType accountType);
    }
}
