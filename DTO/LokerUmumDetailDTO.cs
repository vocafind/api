namespace vocafind_api.DTO
{
    public class LokerUmumDetailDTO
    {
        public string LowonganId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
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
