using MassTransit;
using Messaging.InterfacesConstants.Commands;
using OrdersApi.Persistence.Entities;
using OrdersApi.Persistence.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public RegisterOrderCommandConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            var result = context.Message;

            SaveOrder(result);

            return Task.FromResult(true);
        }

        private void SaveOrder(IRegisterOrderCommand result)
        {
            Order order = new()
            {
                Id = result.Id,
                UserEmail = result.UserEmail,
                PictureUrl = result.PictureUrl,
                ImageData = result.ImageData
            };

            _orderRepository.RegisterOrder(order);
        }
    }
}
