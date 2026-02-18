using CoreBanking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Domain.Entities
{
    public class RevokedToken : BaseEntity
    {
        public string Token { get; set; } = default!;
        public DateTime RevokedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }
    }
}
