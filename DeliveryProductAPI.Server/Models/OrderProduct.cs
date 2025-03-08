using System.ComponentModel.DataAnnotations;

namespace DeliveryProductAPI.Server.Models
{
    public class OrderProduct
    {

        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public int Count { get; set; }

        public OrderProduct() { }
        public OrderProduct(
            Order order,
            Product product,
            int count)
        {
            Order = order;
            Product = product;
            Count = count;
        }
    }
}
