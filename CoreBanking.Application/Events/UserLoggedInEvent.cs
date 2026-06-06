using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Events
{
    public class UserLoggedInEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string IpAddress {  get; set; }
        public DateTime LoginAt { get; set; }
        public string Reference {  get; set; }
    }
}
