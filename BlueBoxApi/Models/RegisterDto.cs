using System.ComponentModel.DataAnnotations;

namespace BlueBoxApi.Models
{
    public class RegisterDto
    {
        [Required] public string DisplayName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        
        [Required]
        //[RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Password must be complex")]
        public string Password { get; set; } = default!;

        [Required]
        public string UserName { get; set; } = default!;
    }
}
