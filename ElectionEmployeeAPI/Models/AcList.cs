using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("ac_list")]
    public class AcList
    {
        [Key]
        [Column("AC_NO")]
        public int AcNo { get; set; }

        [Column("AC_NAME_V1")]
        public string AcNameHindi { get; set; }

        [Column("AC_NAME_En")]
        public string AcNameEnglish { get; set; }

        [Column("DIST_NO")]
        public string DistrictId { get; set; }

        [Column("urban_rural")]
        public string UrbanRural { get; set; }

        [Column("dflag")]
        public bool dflag { get; set; }
    }
}
