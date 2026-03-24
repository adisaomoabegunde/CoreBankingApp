using CoreBanking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = default!;
        public Guid CustomerId { get; set; }
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }
        public Currency Currency { get; set; }
        public AccountStatus Status { get; set; }
        public decimal DailyTransferlimit { get; set; }
        public decimal MinimumBalance { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime? LastTransactionDate {  get; set; }
    }
}
