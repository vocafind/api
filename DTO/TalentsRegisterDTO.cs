using System.ComponentModel.DataAnnotations;

namespace vocafind_api.DTO
{
    public class TalentsRegisterDTO
    {
        [Required, StringLength(255)]
        public string Nama { get; set; }

        [Required, Range(1, 100)]
        public int Usia { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string NomorTelepon { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, RegularExpression("Laki-Laki|Perempuan")]
        public string JenisKelamin { get; set; }

        [Required, StringLength(16, MinimumLength = 16)]
        [RegularExpression("^[0-9]{16}$")]
        public string Nik { get; set; }

        [Required]
        public IFormFile Ktp { get; set; }

    }
}
