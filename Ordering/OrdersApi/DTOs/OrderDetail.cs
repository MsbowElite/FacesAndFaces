using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.DTOs
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public byte[] FaceData { get; set; }
    }
}
