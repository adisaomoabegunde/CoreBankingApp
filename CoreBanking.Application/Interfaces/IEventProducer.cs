using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Interfaces
{
    public interface IEventProducer 
    {
        Task PublishAsync<T>(string topic, T message);  
    }
}
