using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Persistence.Repositories.Interfaces;
using OrdersApi.Results;
using System;
using System.Threading.Tasks;

namespace OrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrdersController(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                return Ok(await _orderRepository.GetOrdersAsync());
            }catch(Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpGet]
        [Route("{orderId}", Name = "GetByOrderId")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            var order = _mapper.Map<GetOrderByIdResult>(await _orderRepository.GetOrderAsync(Guid.Parse(orderId)));

            if (order is not null)
            {
                return Ok(order);
            }
            return NotFound();
        }
    }
}
