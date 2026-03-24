using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Domain.Common.Responses
{
    public class GetAccountDetailsReponse
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }
        public Currency Currency { get; set; }
        public AccountStatus Status { get; set; }
        public decimal DailyTransferLimit { get; set; }
        public decimal MinimumBalance { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime? LasTransactionDate { get; set; }

    }
}
