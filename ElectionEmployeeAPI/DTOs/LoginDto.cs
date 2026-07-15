namespace ElectionEmployeeAPI.DTOs
{
    public class LoginDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Captcha { get; set; } = string.Empty;
        public bool LoginWithOtp { get; set; } = false;
        public string? MobileNumber { get; set; }
    }
}