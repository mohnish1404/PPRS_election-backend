using System.ComponentModel.DataAnnotations;

namespace ElectionEmployeeAPI.DTOs
{
    public class UpdateProfileDto
    {
        [Required]
        [MaxLength(10)]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
    }
}