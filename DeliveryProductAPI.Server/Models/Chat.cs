using System.ComponentModel.DataAnnotations;

namespace DeliveryProductAPI.Server.Models
{
    public class Chat
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }
        public User Client { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public int AdminId { get; set; }
        public User Admin { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();

        public Chat() { }
        public Chat(User client, User admin, Order order)
        {
            Client = client;
            Admin = admin;
            Order = order;
        }
    }
}
