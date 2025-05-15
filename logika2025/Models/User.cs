using Microsoft.AspNetCore.Identity;

namespace logika2025.Models
{
    public class User : IdentityUser
    {

        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

    }
}
