using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.DTOs
{
    public class AuditLogDto
    {
        public Guid? UserId { get; set; }
        public string Action {  get; set; }
        public string IpAddress { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
