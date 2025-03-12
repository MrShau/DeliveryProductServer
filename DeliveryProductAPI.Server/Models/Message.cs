using System.ComponentModel.DataAnnotations;

namespace DeliveryProductAPI.Server.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        [Required]
        public int SenderId { get; set; }
        public User Sender { get; set; }

        [Required]
        public string Content { get; set; }
        public string AttachmentPath { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public Message()
        {
            CreatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time").Id);
            AttachmentPath = string.Empty;
        }

        public Message(Chat chat, User sender, string content) : this()
        {
            Chat = chat;
            Sender = sender;
            Content = content;
        }
    }
}
