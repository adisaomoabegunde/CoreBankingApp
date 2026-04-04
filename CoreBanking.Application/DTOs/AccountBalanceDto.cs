using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.DTOs
{
    public class AccountBalanceDto
    {
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }
    }
}
