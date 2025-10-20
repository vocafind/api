namespace vocafind_api.DTO
{
    public class PerusahaanDTO
    {
        public string CompanyId { get; set; } = null!;

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
    }
}
