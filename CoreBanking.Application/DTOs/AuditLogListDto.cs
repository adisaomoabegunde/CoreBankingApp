using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.DTOs
{
    public class AuditLogListDto
    {
        public List<AuditLogDto> Logs { get; set; }
        public int TotalRecords { get; set; }
    }
}
