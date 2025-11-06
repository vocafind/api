using System.ComponentModel.DataAnnotations;


namespace vocafind_api.DTO
{
    public class TalentsDTO
    {

        //-------------------------------------------------------VERIFIKASI TALENT BY DISNAKER----------------------------
        public class TalentsVerifyDTO
        {
            public string TalentID { get; set; }
            public string StatusAkun { get; set; } // "Belum Terverifikasi", "Sudah Terverifikasi", "Tidak Terverifikasi"

        }



        //-------------------------------------------------------DATA TALENT UNVERIFIED----------------------------
        public class TalentsUnverifiedDTO
        {
            public string talentId { get; set; }
            public string nama { get; set; }
            public string email { get; set; }
            public string nomorTelepon { get; set; }
            public string? nik { get; set; }
            public int usia { get; set; }
            public string statusVerifikasi { get; set; }
            public string statusAkun { get; set; }

        }




        //-------------------------------------------------------REGISTER----------------------------
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


            /*public string? JenisKelamin { get; set; }
            public int? Usia { get; set; }*/
            public int? ProvinsiId { get; set; }
            public string? Provinsi { get; set; }
            public int? KabupatenKotaId { get; set; }
            public string? KabupatenKota { get; set; }

        }


        //-------------------------------------------------------LOGIN----------------------------
        public class TalentsLoginDTO
        {
            public string Email { get; set; }
            public string Password { get; set; }

        }




        //-------------------------------------------------------GET TALENT PROFIL /  data diri----------------------------
        public class TalentsGetDataDiriDTO
        {
            public string? FotoProfil { get; set; }
            public string? Nama { get; set; }
            public string? Nik { get; set; }
            public int Usia { get; set; }
            public string JenisKelamin { get; set; } = null!;
            public string? Provinsi { get; set; }
            public string? KabupatenKota { get; set; }
            public string? Alamat { get; set; }
            public string? NomorTelepon { get; set; }
            public string? LokasiKerjaDiinginkan { get; set; }
            public string? StatusPekerjaanSaatIni { get; set; }
            public int? PreferensiGaji { get; set; }
            public TimeOnly? PreferensiJamKerjaMulai { get; set; }
            public TimeOnly? PreferensiJamKerjaSelesai { get; set; }
            public string? PreferensiPerjalananDinas { get; set; }
        }



        //-------------------------------------------------------UPDATE ----------------------------
        public class TalentsUpdateDTO
        {
            public IFormFile? FotoProfil { get; set; }
            public string? Nama { get; set; }
            public string? Alamat { get; set; }
            public string? NomorTelepon { get; set; }

            public string? LokasiKerjaDiinginkan { get; set; }
            public string? StatusPekerjaanSaatIni { get; set; }
            public int? PreferensiGaji { get; set; }
            public TimeOnly? PreferensiJamKerjaMulai { get; set; }
            public TimeOnly? PreferensiJamKerjaSelesai { get; set; }
            public string? PreferensiPerjalananDinas { get; set; }

        }

    }
}
