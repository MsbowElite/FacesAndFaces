using Microsoft.EntityFrameworkCore;
using OrdersApi.Persistence.Entities;
using OrdersApi.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private OrdersDbContext _context;

        public OrderRepository(OrdersDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderAsync(Guid id)
        {
            return await _context.Orders
                .Include("OrderDetail")
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public Task RegisterOrder(Order order)
        {
            _context.Add(order);
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public void UpdateOrder(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
