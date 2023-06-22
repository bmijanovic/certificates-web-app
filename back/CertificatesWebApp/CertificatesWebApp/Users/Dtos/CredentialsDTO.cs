using System.ComponentModel.DataAnnotations;
using Twilio.Rest.Trunking.V1.Trunk;

namespace CertificatesWebApp.Users.Dtos
{
    public class CredentialsDTO
    {

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not in valid form.")]
        [StringLength(40, ErrorMessage = "Email must be between 5 and 40 characters", MinimumLength = 5)]
        public String Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        public String Password { get; set; }

        public String Token { get; set; }
    }
}
