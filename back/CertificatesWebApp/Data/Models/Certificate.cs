using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{

    [Table("certificates")]
    public class Certificate : IBaseEntity
    {
        //fali signature alg
        //bolje issuer serial number
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("serial_number")]
        public String SerialNumber { get; set; }

        [Column("parent_serial_number")]
        public String? ParentSerialNumber { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("type")]
        public CertificateType Type { get; set; }

        [Column("hash_algorithm")]
        public String HashAlgorithm { get; set; }

        [Column("is_valid")]
        public Boolean IsValid { get; set; }

        [Column("issuer_id")]
        public Guid IssuerId { get; set; }

        [Column("owner_id")]
        public Guid OwnerId { get; set; }

        [Column("attributes")]
        public string Attributes{ get; set; }

        public Certificate(string serialNumber, DateTime startDate, DateTime endDate, CertificateType certificateType, bool isValid, Guid issuerId, Guid ownerId, String hashAlgorithm,string attributes,string parentSerialNumber)
        {
            
            SerialNumber = serialNumber;
            ParentSerialNumber = parentSerialNumber;
            StartDate = startDate;
            EndDate = endDate;
            Type = certificateType;
            IsValid = isValid;
            IssuerId = issuerId;
            OwnerId = ownerId;
            HashAlgorithm = hashAlgorithm;
            Attributes = attributes;
        }

        public Certificate()
        {
        }
    }
}
