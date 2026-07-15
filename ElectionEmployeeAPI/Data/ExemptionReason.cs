using System.ComponentModel.DataAnnotations;

namespace ElectionEmployeeAPI.Data
{
    public class ExemptionReason
    {
        [Key]
        public int ReasonId { get; set; }
        public string? ReasonText { get; set; }
        public bool IsActive { get; set; } = true;
    }
}