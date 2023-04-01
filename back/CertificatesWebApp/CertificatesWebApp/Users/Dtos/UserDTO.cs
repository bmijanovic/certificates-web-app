using Data.Models;
using System.ComponentModel.DataAnnotations;

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
        [StringLength(20, ErrorMessage = "Password must be between 5 and 20 characters", MinimumLength = 5)]
        public String Password { get; set; }
        [Required(ErrorMessage = "Telephone is required.")]
        public String Telephone { get; set; }

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
