using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Events
{
    public class CustomerKycUpdatedEvent
    {
        public Guid CustomerId { get; set; }
        public KycStatus KycStatus { get; set; }
        public string Reference { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
