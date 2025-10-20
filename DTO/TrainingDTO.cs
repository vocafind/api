namespace vocafind_api.DTO
{
    // ✅ Untuk CREATE (POST)
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

    // ✅ Untuk GET (OUTPUT)
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

    // ✅ Untuk PATCH (UPDATE)
    public class TrainingPutDTO
    {
        public string NamaPelatihan { get; set; } = null!;

        public string Penyelenggara { get; set; } = null!;

        public DateOnly TanggalMulai { get; set; }

        public DateOnly TanggalSelesai { get; set; }

        public string LinkSertifikat { get; set; } = null!;

        public string Deskripsi { get; set; } = null!;
    }

}
