using System.ComponentModel.DataAnnotations;

namespace DeliveryProductAPI.Server.Models
{
    public class Status
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public Status() { }
        public Status(string name) => Name = name;
    }
}
