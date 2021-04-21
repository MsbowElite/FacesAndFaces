using MassTransit;
using Messaging.InterfacesConstants.Commands;
using Newtonsoft.Json;
using OrdersApi.Persistence.Entities;
using OrdersApi.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public RegisterOrderCommandConsumer(IOrderRepository orderRepository, IHttpClientFactory httpClientFactory)
        {
            _orderRepository = orderRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            try
            {
                var result = context.Message;

                SaveOrder(result);

                var client = _httpClientFactory.CreateClient();
                var orderDetailData = await GetFacesFromFaceApiAsync(client, result.ImageData, result.Id);
                var faces = orderDetailData.Item1;
                var orderId = orderDetailData.Item2;

                await SaveOrderDetailsAsync(orderId, faces);
            }catch(Exception ex)
            {

            }
        }

        private async Task<Tuple<List<byte[]>, Guid>> GetFacesFromFaceApiAsync(HttpClient client, byte[] imageData, Guid orderId)
        {
            var byteContent = new ByteArrayContent(imageData);
            byteContent.Headers.ContentType = new("application/octet-stream");
            using var response = await client.PostAsync("http://localhost:6000/api/faces?orderId=" + orderId, byteContent);

            string apiResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);
        }

        private async Task SaveOrderDetailsAsync(Guid orderId, List<byte[]> faces)
        {
            var order = await _orderRepository.GetOrderAsync(orderId);

            if(order is not null)
            {
                foreach(var face in faces)
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
                ImageData = result.ImageData
            };

            _orderRepository.RegisterOrder(order);
        }
    }
}
