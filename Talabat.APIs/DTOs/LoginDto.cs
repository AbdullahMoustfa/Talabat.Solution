using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
    public class LoginDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        //[DataType(DataType.Password)] => Allow in MVC Only
        public string Password { get; set; } = null!;
    }
}
