using System.ComponentModel.DataAnnotations;

namespace DeliveryProductAPI.Server.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        [Required]
        public int StatusId { get; set; }
        public Status Status { get; set; }

        public string? Address { get; set; }

        public double DeliveryPrice { get; set; }
        public int DeliveryTime { get; set; }

        public List<OrderProduct> Products { get; set; } = new List<OrderProduct>();

        [Required]
        public DateTime CreatedAt { get; set; }

        public Order()
        {
            CreatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time").Id);
        }

        public Order(
            User user,
            double totalPrice,
            Status status,
            double deliveryPrice,
            int deliveryTime,
            string address) : this()
        {
            User = user;
            TotalPrice = totalPrice + deliveryPrice;
            Status = status;
            DeliveryPrice = deliveryPrice;
            DeliveryTime = deliveryTime;
            Address = address;
        }
    }
}
