using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Events
{
    public class AccountCreatedEvent
    {
        public Guid AccountId { get; set; }
        public Guid CustomerId { get; set; }
        public string AccountNumber { get; set; } = default!;
        public decimal InitialBalance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
