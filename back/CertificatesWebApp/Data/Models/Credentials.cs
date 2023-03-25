using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    [Table("credentials")]
    public class Credentials
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("email")]
        public String Email { get; set; }
        
        [Column("password")]
        public String Password { get; set; }

        [Column("expiration_date")]
        public String ExpiratonDate { get; set; }

        public User User { get; set; }
    }
}
