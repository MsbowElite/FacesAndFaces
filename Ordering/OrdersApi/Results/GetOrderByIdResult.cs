using OrdersApi.DTOs;
using System;
using System.Collections.Generic;

namespace OrdersApi.Results
{
    public class GetOrderByIdResult
    {
        public Guid Id { get; set; }
        public string PictureUrl { get; set; }
        public byte[] ImageData { get; set; }
        public string UserEmail { get; set; }
        public string Status { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
