namespace vocafind_api.Models
{
    public class BlockedIp
    {
        public int Id { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime BlockedAt { get; set; }
        public DateTime BlockedUntil { get; set; }

    }
}
