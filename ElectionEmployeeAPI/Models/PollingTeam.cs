
using System.ComponentModel.DataAnnotations;

namespace ElectionEmployeeAPI.Models
{
    public class PollingTeam
    {
        [Key]
        public int TeamId { get; set; }

        public string District_ID { get; set; }
        public int AC_No { get; set; }
        public int Part_No { get; set; }

        public int DutyBy_UserId { get; set; }
        public DateTime DutyDateTime { get; set; }
    }
}
