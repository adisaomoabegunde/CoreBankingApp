using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.DTOs
{
    public class DailyTransactionSummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalTransactions { get; set; }

        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }
        public decimal TotalTransfers { get; set; }
        
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }

    }
}
