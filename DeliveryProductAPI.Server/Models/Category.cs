using System.ComponentModel.DataAnnotations;

namespace DeliveryProductAPI.Server.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        public bool Status { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();

        public Category() { Status = true; }
        public Category(string name) : this() => this.Name = name;
    }
}
