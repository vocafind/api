namespace vocafind_api.DTO
{
    // ✅ Untuk CREATE (POST)
    public class WorkHistoryPostDTO
    {
        public string TalentId { get; set; } = null!;

        public string Posisi { get; set; } = null!;

        public string Perusahaan { get; set; } = null!;

        public string TanggalMulai { get; set; } = null!;

        public string TanggalSelesai { get; set; } = null!;

        public string Deskripsi { get; set; } = null!;
    }

    // ✅ Untuk GET (OUTPUT)
    public class WorkHistoryGetDTO
    {
        public string WorkhistoryId { get; set; } = null!;

        public string TalentId { get; set; } = null!;

        public string Posisi { get; set; } = null!;

        public string Perusahaan { get; set; } = null!;

        public string TanggalMulai { get; set; } = null!;

        public string TanggalSelesai { get; set; } = null!;

        public string Deskripsi { get; set; } = null!;
    }

    // ✅ Untuk PATCH (UPDATE)
    public class WorkHistoryPutDTO
    {
        public string Posisi { get; set; } = null!;

        public string Perusahaan { get; set; } = null!;

        public string TanggalMulai { get; set; } = null!;

        public string TanggalSelesai { get; set; } = null!;

        public string Deskripsi { get; set; } = null!;
    }

}
