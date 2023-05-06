using Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CertificatesWebApp.Users.Dtos
{
    public class UserDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        public String Surname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(40, ErrorMessage = "Email must be between 5 and 40 characters", MinimumLength = 5)]
        [EmailAddress(ErrorMessage = "Email is not in valid form.")]
        public String Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*_=+-]).{8,16}", ErrorMessage = "Password pattern is not valid (at least one lowercase, one uppercase, one numeric and one symbol).")]
        public String Password { get; set; }

        [Required(ErrorMessage = "Telephone is required.")]
        [RegularExpression("^\\+381\\d{1,2}\\d{3,11}$", ErrorMessage = "Telephone number is not valid.")]
        public String Telephone { get; set; }


        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Required(ErrorMessage = "Verification type is required.")]
        public VerificationType VerificationType { get; set; }

        public UserDTO(User user)
        {
            Name = user.Name;
            Surname = user.Surname;
            Email = user.Email;
            Telephone = user.Telephone;
        }

        public UserDTO()
        {
        }
    }
}
