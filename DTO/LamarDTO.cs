namespace vocafind_api.DTO
{
    // DTO untuk response lamaran
    public class LamarResponseDTO
    {
        public string LamaranID { get; set; }
        public string LowonganID { get; set; }
        public string TalentID { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // DTO untuk detail lamaran saya
    public class LamaranSayaDTO
    {
        public string LamaranID { get; set; }
        public string LowonganID { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public LowonganInfoDTO Lowongan { get; set; }
    }

    public class LowonganInfoDTO
    {
        public string Judul { get; set; }
        public string Deskripsi { get; set; }
        public string Lokasi { get; set; }
        public string Gaji { get; set; }
        public CompanyInfoDTO Company { get; set; }
    }

    public class CompanyInfoDTO
    {
        public string Nama { get; set; }
        public string Logo { get; set; }
    }
}
