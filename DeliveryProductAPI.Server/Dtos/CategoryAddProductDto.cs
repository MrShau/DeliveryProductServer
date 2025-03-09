using DeliveryProductAPI.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace DeliveryProductAPI.Server.Dtos
{
    public class CategoryAddProductDto
    {
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

        public double Weight { get; set; }
        public string WeightUnit { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }
    }
}
