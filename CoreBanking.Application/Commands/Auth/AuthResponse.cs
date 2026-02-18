using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
