namespace vocafind_api.Models
{
    public class LoginAttempt
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public DateTime AttemptTime { get; set; }
        public bool IsSuccess { get; set; }
        public string? UserAgent { get; set; }

    }
}
