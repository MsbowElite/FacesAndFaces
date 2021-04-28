using MassTransit;
using Messaging.InterfacesConstants.Commands;
using Messaging.InterfacesConstants.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrdersApi.Configurations;
using OrdersApi.Enums;
using OrdersApi.Hubs;
using OrdersApi.Persistence.Entities;
using OrdersApi.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHubContext<OrderHub> _orderHubContext;
        private readonly IOptions<OrderSettings> _options;

        public RegisterOrderCommandConsumer(IOrderRepository orderRepository, IHttpClientFactory httpClientFactory, 
            IHubContext<OrderHub> orderHubContext, IOptions<OrderSettings> options)
        {
            _orderRepository = orderRepository;
            _httpClientFactory = httpClientFactory;
            _orderHubContext = orderHubContext;
            _options = options;
        }

        public async Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            var result = context.Message;

            SaveOrder(result);
            await _orderHubContext.Clients.All.SendAsync("UpdateOrders", "New Order Created", result.Id);

            var client = _httpClientFactory.CreateClient();
            var orderDetailData = await GetFacesFromFaceApiAsync(client, result.ImageData, result.Id);
            var faces = orderDetailData.Item1;
            var orderId = orderDetailData.Item2;

            await SaveOrderDetailsAsync(orderId, faces);

            await _orderHubContext.Clients.All.SendAsync("UpdateOrders", "Order processed", result.Id);

            await context.Publish<IOrderProcessedEvent>(new
            {
                Id = orderId,
                result.UserEmail,
                Faces = faces,
                result.PictureUrl
            });
        }

        private async Task<Tuple<List<byte[]>, Guid>> GetFacesFromFaceApiAsync(HttpClient client, byte[] imageData, Guid orderId)
        {
            var byteContent = new ByteArrayContent(imageData);
            byteContent.Headers.ContentType = new("application/octet-stream");
            using var response = await client.PostAsync(
                string.Format("{0}{1}{2}", _options.Value.FacesApiUrl, "/api/faces?orderId=", orderId), byteContent);

            string apiResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);
        }

        private async Task SaveOrderDetailsAsync(Guid orderId, List<byte[]> faces)
        {
            var order = await _orderRepository.GetOrderAsync(orderId);

            if (order is not null)
            {
                foreach (var face in faces)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = orderId,
                        FaceData = face
                    };

                    order.OrderDetails.Add(orderDetail);
                }

                _orderRepository.UpdateOrder(order);
            }
        }

        private void SaveOrder(IRegisterOrderCommand result)
        {
            Order order = new()
            {
                Id = result.Id,
                UserEmail = result.UserEmail,
                PictureUrl = result.PictureUrl,
                ImageData = result.ImageData,
                Status = Status.Registered.ToString()
            };

            _orderRepository.RegisterOrder(order);
        }
    }
}
