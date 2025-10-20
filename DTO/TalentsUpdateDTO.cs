namespace vocafind_api.DTO
{
    public class TalentsUpdateDTO
    {
        public string? FotoProfil { get; set; }
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
