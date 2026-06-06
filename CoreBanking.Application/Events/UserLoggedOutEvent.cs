using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Events
{
    public class UserLoggedOutEvent
    {
        public Guid UserId { get; set; }
        public string IpAddress { get; set; }
        public DateTime LoggedOutAt { get; set; }
        public string Reference { get; set; }
    }
}
