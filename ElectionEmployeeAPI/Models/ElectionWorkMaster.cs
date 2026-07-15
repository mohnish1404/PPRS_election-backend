using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("electionworkmaster")]
    public class ElectionWorkMaster
    {
        [Key]
        [Column("OtherElectionWorkId")]
        public int OtherElectionWorkId { get; set; }

        [Column("ElectionWork_V1")]
        public string ElectionWorkHindi { get; set; } = null!;

        [Column("ElectionWork_En")]
        public string ElectionWorkEnglish { get; set; } = null!;

        [Column("dflag")]
        public bool dflag { get; set; }
    }
}
