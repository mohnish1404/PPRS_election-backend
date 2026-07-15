using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("blocks")]
    public class Block
    {
        // 🔑 Composite Primary Key (Block_ID + District_ID)
        [Column("Block_ID")]
        public string BlockId { get; set; }   // varchar(3)

        [Column("District_ID")]
        public string DistrictId { get; set; } // varchar(2)

        [Column("Block_Name_V1")]
        public string BlockNameHindi { get; set; }

        [Column("Block_Name_En")]
        public string BlockNameEnglish { get; set; }

        [Column("dflag")]
        public bool dflag { get; set; }
    }
}
