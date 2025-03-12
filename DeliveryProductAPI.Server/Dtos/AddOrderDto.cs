using DeliveryProductAPI.Server.Models;
using Microsoft.Identity.Client;

namespace DeliveryProductAPI.Server.Dtos
{
    public class AddOrderDto
    {
        public List<OrderItem> Products { get; set; }
        public double DeliveryPrice { get; set; }
        public int DeliveryTime { get; set; }
        public string Address { get; set; }
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        
    }

}
