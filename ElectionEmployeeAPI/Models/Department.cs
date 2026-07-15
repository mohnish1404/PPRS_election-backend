using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("department")]
    public class Department
    {
        [Key]
        [Column("Dept_Id")]
        public int DeptId { get; set; }

        [Column("Dept")]
        public string DeptHindi { get; set; }

        [Column("DeptEn")]
        public string DeptEnglish { get; set; }

        [Column("dflag")]
        public bool dflag { get; set; }
    }
}
