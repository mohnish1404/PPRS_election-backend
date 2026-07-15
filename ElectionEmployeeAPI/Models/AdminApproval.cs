using System.ComponentModel.DataAnnotations;

namespace ElectionEmployeeAPI.Models
{
    public class AdminApproval
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string RequestType { get; set; } = string.Empty;
        // RequestType: "Register", "ForgotPassword", "ActivateUser"

        public string Status { get; set; } = "Pending";
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public string? Remarks { get; set; }
    }
}