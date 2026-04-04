using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.DTOs
{
    public class AccountStatementDto
    {
        public string AccountNumber { get; set; }
        public decimal CurrentBalance { get; set; }
        public List<TransactionStatementItemDto> Transactions { get; set; }
        public int TotalRecords { get; set; }
    }
}
