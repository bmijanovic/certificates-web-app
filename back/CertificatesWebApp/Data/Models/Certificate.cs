using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{

    [Table("certificates")]
    public class Certificate : IBaseEntity
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("serial_number")]
        public String SerialNumber { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("type")]
        public CertificateType CertificateType { get; set; }

        [Column("is_valid")]
        public Boolean IsValid { get; set; }

        [Column("issuer_id")]
        public Guid IssuerId { get; set; }

        [Column("owner_id")]
        public Guid OwnerId { get; set; }

        [Column("address")]
        public String Address { get; set; }
    }
}
