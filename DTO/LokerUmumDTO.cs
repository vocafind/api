using vocafind_api.Models;

namespace vocafind_api.DTO
{
    public class LokerUmumDTO
    {
        public string LowonganId { get; set; } = null!;
        public string Posisi { get; set; } = null!;
        public string DeskripsiPekerjaan { get; set; } = null!;
        public string? MinimalLulusan { get; set; }
        public string Status { get; set; } = null!;
        public string Lokasi { get; set; } = null!;
        public string Gaji { get; set; } = null!;
        public string JenisPekerjaan { get; set; } = null!;
        public DateOnly TanggalPosting { get; set; }
        public DateOnly BatasLamaran { get; set; }
        public int BatasPelamar { get; set; }
        public int JumlahPelamar { get; set; }
        public string TingkatPengalaman { get; set; } = null!;
        public bool OpsiKerjaRemote { get; set; }
        public string KontrakDurasi { get; set; } = null!;
        public string PeluangKarir { get; set; } = null!;

        //perusahaan
        public string NamaPerusahaan { get; set; } = null!;
        public string? Logo { get; set; }
    }



    public class LokerUmumDetailDTO
    {
        public string LowonganId { get; set; } = null!;
        public string Posisi { get; set; } = null!;
        public string DeskripsiPekerjaan { get; set; } = null!;
        public string? MinimalLulusan { get; set; }
        public string Status { get; set; } = null!;
        public string Lokasi { get; set; } = null!;
        public string Gaji { get; set; } = null!;
        public string JenisPekerjaan { get; set; } = null!;
        public DateOnly TanggalPosting { get; set; }
        public DateOnly BatasLamaran { get; set; }
        public int BatasPelamar { get; set; }
        public int JumlahPelamar { get; set; }
        public string TingkatPengalaman { get; set; } = null!;
        public bool OpsiKerjaRemote { get; set; }
        public string KontrakDurasi { get; set; } = null!;
        public string PeluangKarir { get; set; } = null!;



        //perusahaan
        public string NamaPerusahaan { get; set; } = null!;
        public string? Nib { get; set; }
        public string? Npwp { get; set; }
        public string BidangUsaha { get; set; } = null!;
        public string Alamat { get; set; } = null!;
        public string Provinsi { get; set; } = null!;
        public string Kota { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string NomorTelepon { get; set; } = null!;
        public string? Website { get; set; }
        public string? Logo { get; set; }
        public string? DeskripsiPerusahaan { get; set; }
        public int? JumlahKaryawan { get; set; }
        public string? KebijakanKerja { get; set; }
        public string? BudayaPerusahaan { get; set; }
        public int? JumlahProyekBerjalan { get; set; }




        // Tambahkan properti relasi
        public List<JobQualificationDTO> JobQualifications { get; set; } = new();
        public List<JobBenefitDTO> JobBenefits { get; set; } = new();
        public List<JobAdditionalRequirementDTO> JobAdditionalRequirements { get; set; } = new();
        public List<JobAdditionalFacilityDTO> JobAdditionalFacilities { get; set; } = new();
    }





    // Contoh DTO untuk relasi
    public class JobQualificationDTO
    {
        public string Kualifikasi { get; set; } = null!;
    }

    public class JobBenefitDTO
    {
        public string Benefit { get; set; } = null!;
    }

    public class JobAdditionalRequirementDTO
    {
        public string Persyaratan { get; set; } = null!;
    }

    public class JobAdditionalFacilityDTO
    {
        public string Fasilitas { get; set; } = null!;
    }
}
