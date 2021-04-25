using MassTransit;
using Messaging.InterfacesConstants.Events;
using OrdersApi.Enums;
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
        public OrderDispatchedEventConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Consume(ConsumeContext<IOrderDispatchedEvent> context)
        {
            var message = context.Message;
            Guid orderId = message.Id;
            await UpdateDatabase(orderId);
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
