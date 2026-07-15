using System.ComponentModel.DataAnnotations;

namespace ElectionEmployeeAPI.Models
{
    public class PasswordResetRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Contact { get; set; } = string.Empty;

        public string ContactType { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
    }
}