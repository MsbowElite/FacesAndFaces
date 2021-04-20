using OrdersApi.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Persistence.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderAsync(Guid id);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task RegisterOrder(Order order);
        void UpdateOrder(Order order);
    }
}
