using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("branch_master")]
    public class Branch
    {
        [Key]
        [Column("IFSCode")]
        public string IFSCode { get; set; }

        [Column("BankCode")]
        public int BankCode { get; set; }

        [Column("BranchName_V1")]
        public string BranchNameHindi { get; set; }

        [Column("BranchName_En")]
        public string BranchNameEnglish { get; set; }

        [Column("District_Id")]
        public string DistrictId { get; set; }

        [Column("dflag")]
        public bool dflag { get; set; }
    }
}
