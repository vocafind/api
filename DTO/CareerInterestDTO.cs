namespace vocafind_api.DTO
{

    // ✅ Untuk CREATE (POST)
    public class CareerInterestPostDTO
    {
        public string TalentId { get; set; } = null!;
        public string TingkatKetertarikan { get; set; } = null!;
        public string Alasan { get; set; } = null!;
        public string BidangKetertarikan { get; set; } = null!;
    }

    // ✅ Untuk GET (OUTPUT)
    public class CareerInterestGetDTO
    {
        public string CareerinterestId { get; set; } = null!;   // Diperlukan frontend
        public string TalentId { get; set; } = null!;
        public string TingkatKetertarikan { get; set; } = null!;
        public string Alasan { get; set; } = null!;
        public string BidangKetertarikan { get; set; } = null!;
    }

    // ✅ Untuk PUT
    public class CareerInterestPutDTO
    {
        public string TingkatKetertarikan { get; set; } = null!;
        public string Alasan { get; set; } = null!;
        public string BidangKetertarikan { get; set; } = null!;
    }
}
