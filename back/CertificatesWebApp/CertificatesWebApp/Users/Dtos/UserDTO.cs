using Data.Models;

namespace CertificatesWebApp.Users.Dtos
{
    public class UserDTO
    {
        public String Name { get; set; }
        public String Surname { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
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
