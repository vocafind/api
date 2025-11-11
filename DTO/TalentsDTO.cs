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





        //------------------PROFIL------------------PROFIL------------------PROFIL------------------PROFIL------------------PROFIL------------------PROFIL------------------PROFIL------------------PROFIL------------------PROFIL------------------PROFIL



        //-------------------------------------------------------DATA DIRI----------------------------
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



        //---------------------------------------------------SOSMED----------------------------------------------------
        public class SocialPostDTO
        {
            public string TalentId { get; set; } = null!;
            public string Platform { get; set; } = null!;
            public string Username { get; set; } = null!;
            public string Url { get; set; } = null!;
        }

        public class SocialGetDTO
        {
            public string SocialId { get; set; } = null!;   // Diperlukan frontend
            public string TalentId { get; set; } = null!;
            public string Platform { get; set; } = null!;
            public string Username { get; set; } = null!;
            public string Url { get; set; } = null!;
        }

        public class SocialPutDTO
        {
            public string Platform { get; set; } = null!;
            public string Username { get; set; } = null!;
            public string Url { get; set; } = null!;
        }



        //---------------------------------------------------MINAT KARIR----------------------------------------------------
        public class CareerInterestPostDTO
        {
            public string TalentId { get; set; } = null!;
            public string TingkatKetertarikan { get; set; } = null!;
            public string Alasan { get; set; } = null!;
            public string BidangKetertarikan { get; set; } = null!;
        }

        public class CareerInterestGetDTO
        {
            public string CareerinterestId { get; set; } = null!;   // Diperlukan frontend
            public string TalentId { get; set; } = null!;
            public string TingkatKetertarikan { get; set; } = null!;
            public string Alasan { get; set; } = null!;
            public string BidangKetertarikan { get; set; } = null!;
        }

        public class CareerInterestPutDTO
        {
            public string TingkatKetertarikan { get; set; } = null!;
            public string Alasan { get; set; } = null!;
            public string BidangKetertarikan { get; set; } = null!;
        }


        //---------------------------------------------------REFERENSI----------------------------------------------------
        public class TalentReferencePostDTO
        {
            public string TalentId { get; set; } = null!;

            public string Nama { get; set; } = null!;

            public string Relasi { get; set; } = null!;

            public string? Perusahaan { get; set; }

            public string? Posisi { get; set; }

            public string Email { get; set; } = null!;

            public string Telepon { get; set; } = null!;

            public string? Deskripsi { get; set; }
        }

        public class TalentReferenceGetDTO
        {
            public string ReferenceId { get; set; } = null!;

            public string TalentId { get; set; } = null!;

            public string Nama { get; set; } = null!;

            public string Relasi { get; set; } = null!;

            public string? Perusahaan { get; set; }

            public string? Posisi { get; set; }

            public string Email { get; set; } = null!;

            public string Telepon { get; set; } = null!;

            public string? Deskripsi { get; set; }
        }

        public class TalentReferencePutDTO
        {
            public string Nama { get; set; } = null!;
            public string Relasi { get; set; } = null!;

            public string? Perusahaan { get; set; }

            public string? Posisi { get; set; }

            public string Email { get; set; } = null!;

            public string Telepon { get; set; } = null!;

            public string? Deskripsi { get; set; }
        }






        //------------------AKADEMIK------------------AKADEMIK------------------AKADEMIK------------------AKADEMIK------------------AKADEMIK------------------AKADEMIK------------------AKADEMIK------------------AKADEMIK------------------AKADEMIK------------------

        //---------------------------------------------------PENDIDIKAN----------------------------------------------------
        public class EducationPostDTO
        {
            public string TalentId { get; set; } = null!;

            public string Institusi { get; set; } = null!;

            public string Jurusan { get; set; } = null!;

            public string Jenjang { get; set; } = null!;

            public string? Gelar { get; set; }

            public decimal NilaiAkhir { get; set; }

            public int TahunMasuk { get; set; }

            public int TahunLulus { get; set; }
        }

        public class EducationGetDTO
        {
            public string EducationId { get; set; } = null!;

            public string TalentId { get; set; } = null!;

            public string Institusi { get; set; } = null!;

            public string Jurusan { get; set; } = null!;

            public string Jenjang { get; set; } = null!;

            public string? Gelar { get; set; }

            public decimal NilaiAkhir { get; set; }

            public int TahunMasuk { get; set; }

            public int TahunLulus { get; set; }
        }

        public class EducationPutDTO
        {
            public string Institusi { get; set; } = null!;

            public string Jurusan { get; set; } = null!;

            public string Jenjang { get; set; } = null!;

            public string? Gelar { get; set; }

            public decimal NilaiAkhir { get; set; }

            public int TahunMasuk { get; set; }

            public int TahunLulus { get; set; }
        }


        //---------------------------------------------------BAHASA----------------------------------------------------
        public class LanguagePostDTO
        {
            public string TalentId { get; set; } = null!;

            public string NamaBahasa { get; set; } = null!;

            public string Profisiensi { get; set; } = null!;

            public string? Sertifikat { get; set; }

            public string? Skor { get; set; }
        }

        public class LanguageGetDTO
        {
            public string LanguageId { get; set; } = null!;

            public string TalentId { get; set; } = null!;

            public string NamaBahasa { get; set; } = null!;

            public string Profisiensi { get; set; } = null!;

            public string? Sertifikat { get; set; }

            public string? Skor { get; set; }
        }

        public class LanguagePutDTO
        {
            public string NamaBahasa { get; set; } = null!;

            public string Profisiensi { get; set; } = null!;

            public string? Sertifikat { get; set; }

            public string? Skor { get; set; }
        }



        //---------------------------------------------------PENGHARGAAN----------------------------------------------------
        public class AwardPostDTO
        {
            public string TalentId { get; set; } = null!;

            public string NamaPenghargaan { get; set; } = null!;

            public string TingkatPenghargaan { get; set; } = null!;

            public string PemberiPenghargaan { get; set; } = null!;

            public int Tahun { get; set; }

            public string Deskripsi { get; set; } = null!;

            public string Sertifikat { get; set; } = null!;
        }

        public class AwardGetDTO
        {
            public string AwardId { get; set; } = null!;

            public string TalentId { get; set; } = null!;

            public string NamaPenghargaan { get; set; } = null!;

            public string TingkatPenghargaan { get; set; } = null!;

            public string PemberiPenghargaan { get; set; } = null!;

            public int Tahun { get; set; }

            public string Deskripsi { get; set; } = null!;

            public string Sertifikat { get; set; } = null!;
        }

        public class AwardPutDTO
        {
            public string NamaPenghargaan { get; set; } = null!;

            public string TingkatPenghargaan { get; set; } = null!;

            public string PemberiPenghargaan { get; set; } = null!;

            public int Tahun { get; set; }

            public string Deskripsi { get; set; } = null!;

            public string Sertifikat { get; set; } = null!;
        }









        //------------------KOMPETENSI------------------KOMPETENSI------------------KOMPETENSI------------------KOMPETENSI------------------KOMPETENSI------------------KOMPETENSI------------------KOMPETENSI------------------KOMPETENSI------------------KOMPETENSI------------------KOMPETENSI

        //---------------------------------------------------SERTIFIKASI----------------------------------------------------
        public class CertificationPostDTO
        {
            public string TalentId { get; set; } = null!;

            public string NamaSertifikasi { get; set; } = null!;

            public string LembagaSertifikasi { get; set; } = null!;

            public DateOnly TanggalTerbit { get; set; }

            public DateOnly TanggalHabisMasa { get; set; }

            public string? NomorSertifikat { get; set; }

            public string Sertifikat { get; set; } = null!;
        }

        public class CertificationGetDTO
        {
            public string CertificationId { get; set; } = null!;

            public string TalentId { get; set; } = null!;

            public string NamaSertifikasi { get; set; } = null!;

            public string LembagaSertifikasi { get; set; } = null!;

            public DateOnly TanggalTerbit { get; set; }

            public DateOnly TanggalHabisMasa { get; set; }

            public string? NomorSertifikat { get; set; }

            public string Sertifikat { get; set; } = null!;
        }

        public class CertificationPutDTO
        {
            public string NamaSertifikasi { get; set; } = null!;

            public string LembagaSertifikasi { get; set; } = null!;

            public DateOnly TanggalTerbit { get; set; }

            public DateOnly TanggalHabisMasa { get; set; }

            public string? NomorSertifikat { get; set; }

            public string Sertifikat { get; set; } = null!;
        }



        //---------------------------------------------------PELATIHAN----------------------------------------------------
        public class TrainingPostDTO
        {
            public string TalentId { get; set; } = null!;

            public string NamaPelatihan { get; set; } = null!;

            public string Penyelenggara { get; set; } = null!;

            public DateOnly TanggalMulai { get; set; }

            public DateOnly TanggalSelesai { get; set; }

            public string LinkSertifikat { get; set; } = null!;

            public string Deskripsi { get; set; } = null!;
        }

        public class TrainingGetDTO
        {
            public string TrainingId { get; set; } = null!;

            public string TalentId { get; set; } = null!;

            public string NamaPelatihan { get; set; } = null!;

            public string Penyelenggara { get; set; } = null!;

            public DateOnly TanggalMulai { get; set; }

            public DateOnly TanggalSelesai { get; set; }

            public string LinkSertifikat { get; set; } = null!;

            public string Deskripsi { get; set; } = null!;
        }

        public class TrainingPutDTO
        {
            public string NamaPelatihan { get; set; } = null!;

            public string Penyelenggara { get; set; } = null!;

            public DateOnly TanggalMulai { get; set; }

            public DateOnly TanggalSelesai { get; set; }

            public string LinkSertifikat { get; set; } = null!;

            public string Deskripsi { get; set; } = null!;
        }




        //---------------------------------------------------SOFT SKILL----------------------------------------------------
        public class SoftSkillPostDTO
        {
            public string TalentId { get; set; } = null!;

            public string NamaSkill { get; set; } = null!;

            public string Profisiensi { get; set; } = null!;

            public string Deskripsi { get; set; } = null!;
        }

        public class SoftSkillGetDTO
        {
            public string SoftskillsId { get; set; } = null!;

            public string TalentId { get; set; } = null!;

            public string NamaSkill { get; set; } = null!;

            public string Profisiensi { get; set; } = null!;

            public string Deskripsi { get; set; } = null!;
        }

        public class SoftSkillPutDTO
        {
            public string NamaSkill { get; set; } = null!;

            public string Profisiensi { get; set; } = null!;

            public string Deskripsi { get; set; } = null!;
        }








        //------------------PENGALAMAN------------------PENGALAMAN------------------PENGALAMAN------------------PENGALAMAN------------------PENGALAMAN------------------PENGALAMAN------------------PENGALAMAN------------------PENGALAMAN------------------PENGALAMAN

        //---------------------------------------------------RIWAYAT PEKERJAAN----------------------------------------------------

        public class WorkHistoryPostDTO
        {
            public string TalentId { get; set; } = null!;

            public string Posisi { get; set; } = null!;

            public string Perusahaan { get; set; } = null!;

            public DateOnly TanggalMulai { get; set; }

            public DateOnly TanggalSelesai { get; set; }

            public string Deskripsi { get; set; } = null!;
        }

        public class WorkHistoryGetDTO
        {
            public string WorkhistoryId { get; set; } = null!;

            public string TalentId { get; set; } = null!;

            public string Posisi { get; set; } = null!;

            public string Perusahaan { get; set; } = null!;

            public DateOnly TanggalMulai { get; set; }

            public DateOnly TanggalSelesai { get; set; }

            public string Deskripsi { get; set; } = null!;
        }

        public class WorkHistoryPutDTO
        {
            public string Posisi { get; set; } = null!;

            public string Perusahaan { get; set; } = null!;

            public DateOnly TanggalMulai { get; set; }

            public DateOnly TanggalSelesai { get; set; }

            public string Deskripsi { get; set; } = null!;
        }



        //---------------------------------------------------PROYEK----------------------------------------------------
        public class ProjectPostDTO
        {
            public string TalentId { get; set; } = null!;

            public string NamaProyek { get; set; } = null!;

            public string Klien { get; set; } = null!;

            public DateOnly TanggalMulai { get; set; }

            public DateOnly TanggalSelesai { get; set; }

            public string PeranTim { get; set; } = null!;

            public string PenggunaanTeknologi { get; set; } = null!;
        }

        public class ProjectGetDTO
        {
            public string ProjectId { get; set; } = null!;

            public string TalentId { get; set; } = null!;

            public string NamaProyek { get; set; } = null!;

            public string Klien { get; set; } = null!;

            public DateOnly TanggalMulai { get; set; }

            public DateOnly TanggalSelesai { get; set; }

            public string PeranTim { get; set; } = null!;

            public string PenggunaanTeknologi { get; set; } = null!;
        }

        public class ProjectPutDTO
        {
            public string NamaProyek { get; set; } = null!;

            public string Klien { get; set; } = null!;

            public DateOnly TanggalMulai { get; set; }

            public DateOnly TanggalSelesai { get; set; }

            public string PeranTim { get; set; } = null!;

            public string PenggunaanTeknologi { get; set; } = null!;
        }



        //---------------------------------------------------Portofolio----------------------------------------------------
        public class PortofolioPostDTO
        {
            public string TalentId { get; set; } = null!;

            public string Judul { get; set; } = null!;

            public string Deskripsi { get; set; } = null!;

            public string? LinkPorotofolio { get; set; }

            public string? GaleriPortofolio { get; set; }

        }

        public class PortofolioGetDTO
        {
            public string PortfolioId { get; set; } = null!;

            public string TalentId { get; set; } = null!;

            public string Judul { get; set; } = null!;

            public string Deskripsi { get; set; } = null!;

            public string? LinkPorotofolio { get; set; }

            public string? GaleriPortofolio { get; set; }
        }

        public class PortofolioPutDTO
        {
            public string Judul { get; set; } = null!;

            public string Deskripsi { get; set; } = null!;

            public string? LinkPorotofolio { get; set; }

            public string? GaleriPortofolio { get; set; }
        }


    }



}
