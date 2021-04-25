using System;

namespace OrdersApi.DTOs
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public byte[] FaceData { get; set; }
    }
}
