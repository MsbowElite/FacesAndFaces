using System;

#nullable disable

namespace OrdersApi.Persistence.Entities
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public byte[] FaceData { get; set; }

        public virtual Order Order { get; set; }
    }
}
