using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryProductAPI.Server.Dtos
{
    public class SignInDto
    {

        [AllowNull]
        [DataType(DataType.EmailAddress)]
        public string? EmailAddress { get; set; }


        [AllowNull]
        [StringLength(64, MinimumLength = 3, ErrorMessage = "Длина логина некорректна")]
        public string? Login { get; set; }

        [Required]
        [StringLength(512, MinimumLength = 7, ErrorMessage = "Некорректная длина пароля")]
        public string Password { get; set; }
    }
}
