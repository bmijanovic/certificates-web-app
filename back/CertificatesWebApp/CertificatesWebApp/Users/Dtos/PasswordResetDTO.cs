using System.ComponentModel.DataAnnotations;

namespace CertificatesWebApp.Users.Dtos
{
    public class PasswordResetDTO
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, ErrorMessage = "Password must be between 5 and 20 characters", MinimumLength = 5)]
        public String Password { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password confirmation is required.")]
        [StringLength(20, ErrorMessage = "Password confirmation must be between 5 and 20 characters", MinimumLength = 5)]
        public String PasswordConfirmation { get; set; }
    }
}
