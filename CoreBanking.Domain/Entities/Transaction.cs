using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string TransactionReference { get; set; }
        public TransactionType TransactionType { get; set; }

        public Guid? SourceAccountId { get; set; }
        public Account? SourceAccount { get; set; }
        public Guid? DestinationAccountId { get; set; }
        public Account? DestinationAccount { get; set; }

        public decimal Amount {  get; set; }
        public decimal BalanceAfter { get; set; }

        public string Description { get; set; }
        public TransactionStatus Status { get; set; }

        public string IdempotencyKey { get; set; }
        public Guid ProcessedBy { get; set; }

        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? ReversedTransactionId { get; set; }
        public Transaction? ReversedTransaction { get; set; }
        public bool IsReversed { get; set; } = false;

        public ICollection<LedgerEntry> LedgerEntries { get; set; }

    }
}
