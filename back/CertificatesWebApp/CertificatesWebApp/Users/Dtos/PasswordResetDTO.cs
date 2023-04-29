using System.ComponentModel.DataAnnotations;

namespace CertificatesWebApp.Users.Dtos
{
    public class PasswordResetDTO
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*_=+-]).{8,16}", ErrorMessage = "Password pattern is not valid (at least one lowercase, one uppercase, one numeric and one symbol).")]
        public String Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password confirmation is required.")]
        [RegularExpression("(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*_=+-]).{8,16}", ErrorMessage = "Password pattern is not valid (at least one lowercase, one uppercase, one numeric and one symbol).")]
        public String PasswordConfirmation { get; set; }
    }
}
