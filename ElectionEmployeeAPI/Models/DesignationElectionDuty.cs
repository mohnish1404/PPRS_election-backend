using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("designation_election_duty")]
    public class DesignationElectionDuty
    {
        [Key]
        [Column("Designation_ID")]
        public int DesignationId { get; set; }

        [Column("Designation")]
        public string DesignationHindi { get; set; }

        [Column("Designation_En")]
        public string DesignationEnglish { get; set; }

        [Column("VargID")]
        public string VargId { get; set; }

        [Column("dflag")]
        public bool dflag { get; set; }
    }
}
