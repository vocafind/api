namespace vocafind_api.DTO
{
    public class LokerUmumDTO
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


    }
}
