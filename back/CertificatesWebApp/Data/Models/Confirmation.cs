using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    [Table("activations")]
    public class Confirmation
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("code")]
        public String Code { get; set; }

        [Column("expiration_date")]
        public String ExpirationDate { get; set; }

        [Column("confirmation_type")]
        public ConfirmationType ConfirmationType { get; set; }

    }
}
