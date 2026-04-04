using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Domain.Entities
{
    public class LedgerEntry
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid TransactionId { get; set; }

        public EntryType EntryType { get; set; }
        public decimal Amount { get; set; }

        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }

        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public Transaction Transaction { get; set; }
    }
}
