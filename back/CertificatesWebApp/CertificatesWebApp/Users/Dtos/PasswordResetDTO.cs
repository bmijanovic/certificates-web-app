using System.ComponentModel.DataAnnotations;

namespace CertificatesWebApp.Users.Dtos
{
    public class PasswordResetDTO
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "Password must be between 8 and 50 characters", MinimumLength = 8)]
        public String Password { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password confirmation is required.")]
        [StringLength(50, ErrorMessage = "Password confirmation must be between 8 and 50 characters", MinimumLength = 8)]
        public String PasswordConfirmation { get; set; }
    }
}
