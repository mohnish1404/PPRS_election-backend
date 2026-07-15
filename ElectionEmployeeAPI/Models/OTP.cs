using System.ComponentModel.DataAnnotations;

namespace ElectionEmployeeAPI.Models
{
    public class OTP
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string OtpCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddSeconds(30);
        public bool IsUsed { get; set; } = false;

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }
}