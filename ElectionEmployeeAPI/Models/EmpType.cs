using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("emp_type_master")]
    public class EmpType
    {
        [Key]
        [Column("EmpTypeId")]
        public int EmpTypeId { get; set; }

        [Column("EmpType_V1")]
        public string EmpTypeHindi { get; set; }

        [Column("EmpType_En")]
        public string EmpTypeEnglish { get; set; }

        [Column("dflag")]
        public bool dflag { get; set; }
    }
}
