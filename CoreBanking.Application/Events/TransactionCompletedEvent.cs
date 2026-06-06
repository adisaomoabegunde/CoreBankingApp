using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Events
{
    public class TransactionCompletedEvent
    {
        public Guid TransactionId { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime OccuredAt { get; set; }
    }
}
