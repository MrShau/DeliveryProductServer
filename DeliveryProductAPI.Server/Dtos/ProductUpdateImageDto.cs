using System.ComponentModel.DataAnnotations;

namespace DeliveryProductAPI.Server.Dtos
{
    public class ProductUpdateImageDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }
    }
}
