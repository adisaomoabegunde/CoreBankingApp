using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Events
{
    public class TransactionReversedEvent
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public DateTime ReversedAt { get; set; }
        public TransactionType TransactionType { get; set; }

    }
}
