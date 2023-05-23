using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace CertificatesWebApp.Users.Dtos
{
    public class PasswordResetDTO
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\w\\s]).{12,30}$", ErrorMessage = "Password pattern is not valid (at least one lowercase, one uppercase, one numeric and one symbol).")]
        public String Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password confirmation is required.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\w\\s]).{12,30}$", ErrorMessage = "Password pattern is not valid (at least one lowercase, one uppercase, one numeric and one symbol).")]
        public String PasswordConfirmation { get; set; }
    }
}
