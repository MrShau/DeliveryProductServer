using System.ComponentModel.DataAnnotations;

namespace DeliveryProductAPI.Server.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<User> Users { get; set; } = new List<User>();

        public Role() { }
        public Role(string name) => this.Name = name;
    }
}
