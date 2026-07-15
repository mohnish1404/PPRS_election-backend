using System.ComponentModel.DataAnnotations;

namespace ElectionEmployeeAPI.Models
{
    public class PollingTeamMember
    {
        [Key]
        public int Id { get; set; }

        public int TeamId { get; set; }
        public int? EmpCode { get; set; }

        public int DutyPostId { get; set; }
        public int? ElectionDesignation_Id { get; set; }
    }
}
