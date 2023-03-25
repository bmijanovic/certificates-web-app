using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    [Table("requests")]
    public class CertificateRequest
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("state")]
        public CertificateRequestState State { get; set; }

        [Column("rejection_reason")]
        public String? RejectionReason { get; set; }

        [Column("parent_serial_number")]
        public String ParentSerialNumber { get; set; }

        public User User { get; set; }

    }
}
