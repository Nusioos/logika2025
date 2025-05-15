using System.ComponentModel.DataAnnotations;

namespace logika2025.Models
{
    public class ResetPasswordViewModel
    {

        public string Token { get; set; }

        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

      
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Hasła nie są takie same.")]
        public string ConfirmPassword { get; set; }
    }
}
