using Faces.WebMvc.ViewModels;
using Microsoft.Extensions.Configuration;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Faces.WebMvc.RestClients
{
    public class OrderManagementApi : IOrderManagementApi
    {
        private IOrderManagementApi _restClient;

        public OrderManagementApi(IConfiguration configuration, HttpClient httpClient)
        {
            string apiHostAndPort = configuration.GetSection("ApiServiceLocations")
                .GetValue<string>("OrdersApiLocation");
            httpClient.BaseAddress = new($"http://{apiHostAndPort}/api");

            _restClient = RestService.For<IOrderManagementApi>(httpClient);
        }

        public async Task<OrderViewModel> GetOrderById(Guid orderId)
        {
            try
            {
            return await _restClient.GetOrderById(orderId);

            }
            catch(ApiException ex)
            {
                if(ex.StatusCode is HttpStatusCode.NotFound)
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
