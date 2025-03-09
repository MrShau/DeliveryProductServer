using System.ComponentModel.DataAnnotations;

namespace DeliveryProductAPI.Server.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "Длина названия некорректна")]
        public string Title { get; set; }

        [Required]
        [StringLength(512, MinimumLength = 2, ErrorMessage = "Длина описания некорректна")]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public double Weight { get; set; }
        public string WeightUnit { get; set; }

        public int Count { get; set; }

        [Required]
        public string ImagePath { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public bool Status { get; set; }

        public Product()
        {
            CreatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time").Id);
            Status = true;
        }

        public Product(
            string title,
            string description,
            double price,
            Category category,
            string imagePath,
            double weight,
            string weightUnit,
            int count) : this()
        {
            Title = title;
            Description = description;
            Price = price;
            Category = category;
            ImagePath = imagePath;
            Weight = weight;
            WeightUnit = weightUnit;
            Count = count;
        }
    }
}
