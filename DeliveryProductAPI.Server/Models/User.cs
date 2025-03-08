using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System;

namespace DeliveryProductAPI.Server.Models
{
    public class User
    {
        public int Id { get; set; }

        [AllowNull]
        [DataType(DataType.EmailAddress)]
        public string? EmailAddress { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 3, ErrorMessage = "Длина логина некорректна")]
        public string Login { get; set; }

        [Required]
        [StringLength(512)]
        public string PasswordHash { get; set; }

        [Required]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [Required]
        public bool Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public List<Chat> ChatsAsClient { get; set; } = new List<Chat>();
        public List<Chat> ChatsAsAdmin { get; set; } = new List<Chat>();

        public List<Order> Orders { get; set; } = new List<Order>();

        public User()
        {
            CreatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time").Id);
            Status = true;
        }
        public User(
            string? emailAddress,
            string login,
            string password,
            Role role) : this()
        {
            EmailAddress = emailAddress?.ToLower();
            Login = login.ToLower();
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            Role = role;
        }

        public void SetPassword(string password)
        {
            if (password.Length > 6)
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password) => BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }
}
