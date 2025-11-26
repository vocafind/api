using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vocafind_api.Models
{
    [Table("acara_qr")]
    public class AcaraQr
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Column("application_code")]
        [StringLength(255)]
        public string ApplicationCode { get; set; }

        [Column("applyID")]
        [StringLength(36)]
        public string ApplyId { get; set; }

        [Column("registration_code")]
        [StringLength(255)]
        public string RegistrationCode { get; set; }

        [Column("qr_code_path")]
        [StringLength(255)]
        public string QrCodePath { get; set; }

        // ✅ Tambahkan [NotMapped] agar EF tidak membuat kolom
        [NotMapped]
        public virtual JobApply JobApply { get; set; }

        [NotMapped]
        public virtual TalentAcaraRegistration TalentRegistration { get; set; }
    }
}