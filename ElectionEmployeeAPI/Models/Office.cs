using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("office")]
    public class Office
    {
        [Key]
        [Column("Office_ID")]
        public int OfficeId { get; set; }

        [Column("Office")]
        public string OfficeHindi { get; set; } = null!;

        [Column("Office_En")]
        public string OfficeEnglish { get; set; } = null!;

        [Column("Dept_Id")]
        public int DeptId { get; set; }

        [Column("District_Id")]
        public string DistrictId { get; set; } = null!;   // varchar(2)

        [Column("Block_ID")]
        public string? BlockId { get; set; }              // varchar(3), nullable

        [Column("dflag")]
        public bool dflag { get; set; }
    }
}
