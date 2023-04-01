using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    [Table("requests")]
    public class CertificateRequest : IBaseEntity
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("state")]
        public CertificateRequestState State { get; set; }

        [Column("type")]
        public CertificateType Type { get; set; }

        [Column("rejection_reason")]
        public String? RejectionReason { get; set; }

        [Column("parent_serial_number")]
        public String ParentSerialNumber { get; set; }

        [Column("subject_text")]
        public String SubjectText { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("flags")]
        public String Flags { get; set; }

        [Column("hash_algorithm")]
        public String HashAlgorithm { get; set; }

        [Column("owner_id")]
        public Guid OwnerId { get; set; }

        //public User User { get; set; }

    }
}
