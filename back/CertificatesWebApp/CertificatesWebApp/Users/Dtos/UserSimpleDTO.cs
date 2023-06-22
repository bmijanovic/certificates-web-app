using Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CertificatesWebApp.Users.Dtos
{
    public class UserSimpleDTO
    {
        public String Name { get; set; }
        public String Surname { get; set; }
        public String Email { get; set; }
        public String Telephone { get; set; }


        public UserSimpleDTO(User user)
        {
            Name = user.Name;
            Surname = user.Surname;
            Email = user.Email;
            Telephone = user.Telephone;
        }
    }
}
