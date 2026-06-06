using CoreBanking.Application.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services.Interfaces
{
    public interface IAccountStatusNotificationService
    {
        Task HandleAsync(AccountStatusChangedEvent accountStatusChangedEvent);
    }
}
