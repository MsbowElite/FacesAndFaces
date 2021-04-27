using MassTransit;
using Messaging.InterfacesConstants.Events;
using Microsoft.AspNetCore.SignalR;
using OrdersApi.Enums;
using OrdersApi.Hubs;
using OrdersApi.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class OrderDispatchedEventConsumer : IConsumer<IOrderDispatchedEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHubContext<OrderHub> _orderHubContext;

        public OrderDispatchedEventConsumer(IOrderRepository orderRepository, IHubContext<OrderHub> orderHubContext)
        {
            _orderRepository = orderRepository;
            _orderHubContext = orderHubContext;
        }

        public async Task Consume(ConsumeContext<IOrderDispatchedEvent> context)
        {
            var message = context.Message;
            Guid orderId = message.Id;
            await UpdateDatabase(orderId);
            await _orderHubContext.Clients.All.SendAsync("UpdateOrders", "Order Dispatched", orderId);
        }

        private async Task UpdateDatabase(Guid orderId)
        {
            var order = await _orderRepository.GetOrderAsync(orderId);
            if (order != null)
            {
                order.Status = Status.Sent.ToString();
                _orderRepository.UpdateOrder(order);
            }
        }
    }
}
