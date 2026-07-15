using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("bank_master")]
    public class Bank
    {
        [Key]
        [Column("BankCode")]
        public int BankCode { get; set; }

        [Column("Bank_Name_V1")]
        public string BankNameHindi { get; set; }

        [Column("Bank_Name_En")]
        public string BankNameEnglish { get; set; }

        [Column("dflag")]
        public bool dflag { get; set; }
    }
}
