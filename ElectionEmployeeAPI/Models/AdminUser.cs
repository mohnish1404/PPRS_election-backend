using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("admin_users")]
    public class AdminUser
    {
        [Key]
        public int AdminId { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public string? District_ID { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
