using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("district")]
    public class District
    {
        [Key]
        [Column("District_ID")]
        public string DistrictId { get; set; }   // varchar(2)

        [Column("District_Name_V1")]
        public string DistrictNameHindi { get; set; }

        [Column("District_Name_En")]
        public string DistrictNameEnglish { get; set; }

        [Column("dflag")]
        public bool dflag { get; set; }
    }
}
