using System.ComponentModel.DataAnnotations;

namespace DeliveryProductAPI.Server.Dtos
{
    public class AddCategoryDto
    {
        [Required]
        [StringLength(64, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
