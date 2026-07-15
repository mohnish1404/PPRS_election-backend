using System;
using System.ComponentModel.DataAnnotations;

namespace ElectionEmployeeAPI.Data
{
    public class UserAuditLog
    {
        [Key]
        public int LogId { get; set; }
        public string TargetUserId { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty; // "Activated" | "Deactivated"
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; } = DateTime.Now;
        public string? Remarks { get; set; }
    }
}