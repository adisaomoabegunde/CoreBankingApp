using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Events
{
    public class WithdrawalCompletedEvent
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal NewBalance { get; set; }
        public string Reference { get; set; } = default!;
        public DateTime OccuredAt { get; set; } = DateTime.UtcNow;
    }
}
