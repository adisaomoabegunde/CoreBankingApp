using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Events
{
    public class AccountStatusChangedEvent
    {
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; }
        public AccountStatus OldStatus { get; set; }
        public AccountStatus NewStatus { get; set; }
        public string Reference { get; set; }
        public DateTime OccuredAt { get; set; } = DateTime.UtcNow;
    }
}
