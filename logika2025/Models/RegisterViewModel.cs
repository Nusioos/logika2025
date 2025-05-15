using System.ComponentModel.DataAnnotations;

namespace logika2025.Models
{
    public class RegisterViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Hasła muszą być identyczne.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
