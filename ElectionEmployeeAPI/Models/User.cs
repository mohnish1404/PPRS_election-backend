using System.ComponentModel.DataAnnotations;

namespace ElectionEmployeeAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = false;
        public bool IsApproved { get; set; } = false;
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime PasswordChangedAt { get; set; } = DateTime.UtcNow;

        public bool IsPasswordExpired => (DateTime.UtcNow - PasswordChangedAt).TotalDays > 365;
        public DateTime? ApprovedAt { get; set; }

    }
}