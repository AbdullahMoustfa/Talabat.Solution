using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace Talabat.APIs.DTOs
{
    public class UserDto
    {

        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;

    }
}
