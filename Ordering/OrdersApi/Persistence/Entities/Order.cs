using System;
using System.Collections.Generic;

#nullable disable

namespace OrdersApi.Persistence.Entities
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public string PictureUrl { get; set; }
        public byte[] ImageData { get; set; }
        public string UserEmail { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
