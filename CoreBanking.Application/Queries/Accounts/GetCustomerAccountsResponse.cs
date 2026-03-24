using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Queries.Accounts
{
    public class GetCustomerAccountsResponse
    {
        public string AccountNumber {  get; set; }
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }
        public Currency Currency { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime DateOpened { get; set; }
    }
}
