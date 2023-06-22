using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    [Table("password_records")]
    public class PasswordRecord : IBaseEntity
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Guid Id { get; set; }
        [Column("password")]
        public string Password { get; set; }

        
        [Column("user_id")]
        public User User { get; set; }


        [Column("date_changed")]
        public DateTime DateChanged { get; set; }
    }
}
