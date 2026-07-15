using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("duty_removal_log")]
    public class DutyRemovalLog
    {
        [Key]
        public int Id { get; set; }
        public int EmpCode { get; set; }
        public int TeamId { get; set; }
        public int? AC_No { get; set; }
        public int? Part_No { get; set; }
        public int RemovedBy { get; set; }
        public DateTime RemovedAt { get; set; }
        public string? Remarks { get; set; }

        [ForeignKey("EmpCode")]
        public PollingPersonnel? Employee { get; set; }

        [ForeignKey("TeamId")]
        public PollingTeam? Team { get; set; }
    }
}