using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("duty_post")]
    public class DutyPost
    {
        [Key]
        [Column("DutyPostId")]
        public int DutyPostId { get; set; }

        [Column("DutyPost_V1")]
        public string DutyPostHindi { get; set; }

        [Column("DutyPost_En")]
        public string DutyPostEnglish { get; set; }

        [Column("dflag")]
        public bool dflag { get; set; }
    }
}
