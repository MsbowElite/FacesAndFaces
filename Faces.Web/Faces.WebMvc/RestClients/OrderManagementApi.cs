using Faces.WebMvc.Configurations;
using Faces.WebMvc.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Faces.WebMvc.RestClients
{
    public class OrderManagementApi : IOrderManagementApi
    {
        private readonly IOrderManagementApi _restClient;
        private readonly IOptions<AppSettings> _options;

        public OrderManagementApi(HttpClient httpClient, IOptions<AppSettings> options)
        {
            httpClient.BaseAddress = new($"{options.Value.OrdersApiUrl}/api");

            _restClient = RestService.For<IOrderManagementApi>(httpClient);
            _options = options;
        }

        public async Task<OrderViewModel> GetOrderById(Guid orderId)
        {
            try
            {
                return await _restClient.GetOrderById(orderId);

            }
            catch (ApiException ex)
            {
                if (ex.StatusCode is HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<List<OrderViewModel>> GetOrders()
        {
            return await _restClient.GetOrders();
        }
    }
}
