using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectionEmployeeAPI.Models
{
    [Table("pollingpersonnel")]
    public class PollingPersonnel
    {
        [Key]
        public int? EmpCode { get; set; }

        // -------- Names --------
        public string EmpName { get; set; } = null!;
        public string? EmpName_En { get; set; }
        public string? SurName { get; set; }
        public string? SurName_En { get; set; }

        // -------- Personal --------
        public DateTime DOB { get; set; }

        [Column("SexID")]
        public string SexId { get; set; } = null!;

        public string MobileNo { get; set; } = null!;
        public string? EmpImagePath { get; set; }
     



        // -------- EPIC --------
        public bool HasEPIC { get; set; }
        public string? EPICNo { get; set; }
        public bool EPICVerified { get; set; }
        [Column("EPIC_District_ID")]
        public string? EPIC_District_ID { get; set; }

        [Column("EPIC_Block_ID")]
        public string? EPIC_Block_ID { get; set; }

        [Column("EPIC_Urban_Rural")]
        public string? EPIC_Urban_Rural { get; set; }

        // -------- PWD --------
       

        [Column("PWDTypeId")]
        public int? PWDTypeId { get; set; }
        public int? PWDPercentage { get; set; }
        public string? PWDCertificatePath { get; set; }


        // -------- Address --------
        [Column("HomeDistrict_ID")]
        public string? HomeDistrictId { get; set; }

        [Column("HomeBlock_ID")]
        public string? HomeBlockId { get; set; }

        public string? UrbanRural { get; set; }

        [Column("WorkDistrict_ID")]
        public string WorkDistrictId { get; set; } = null!;

        [Column("WorkBlock_ID")]
        public string? WorkBlockId { get; set; }

        [Column("WorkUrbanRural")]
        public string? WorkUrbanRural { get; set; }

        // -------- Office --------
        [Column("Dept_Id")]
        public int DeptId { get; set; }

        public int Office_ID { get; set; }
        public int? Designation_Id { get; set; }
        [Column("VargId")]
        public string? VargId { get; set; }

        public string? ReservationCategory { get; set; }
        public int? EmpTypeId { get; set; }
        public decimal? Salary { get; set; }

        // -------- Assembly --------
        public int? ResAC { get; set; }
        public int? WorkAC { get; set; }

        // -------- Bank --------
        public int? BankCode { get; set; }
        public string? IFSCode { get; set; }
        public string? AccountNumber { get; set; }
        [Column("AC_No")]
        public int? AC_No { get; set; }

        [Column("Part_No")]
        public int? Part_No { get; set; }

        [Column("Serial_No")]
        public int? Serial_No { get; set; }


        // -------- Experience --------
        public string? ExperiencePolling { get; set; }
        public string? ExperienceCounting { get; set; }

        [Column("isFieldDuty")]
        public string? IsFieldDuty { get; set; }

        [Column("Field_AC")]
        public int? Field_AC { get; set; }

        [Column("Field_District_ID")]
        public string? Field_District_ID { get; set; }

        [Column("Field_Block_ID")]
        public string? Field_Block_ID { get; set; }

        // -------- Duty --------
        //public int? OtherElectionWorkId { get; set; }
        public int? DutyPostId { get; set; }
        public int? ElectionDesignation_Id { get; set; }

        // -------- System --------
        //public DateTime? EntryDt { get; set; }
        public DateTime? UpdateDt { get; set; }
        public bool dflag { get; set; }
    }
}
