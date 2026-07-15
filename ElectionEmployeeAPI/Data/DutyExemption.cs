using System;
using System.ComponentModel.DataAnnotations;

namespace ElectionEmployeeAPI.Data
{
    public class DutyExemption
    {
        [Key]
        public int ExemptionId { get; set; }
        public int MemberId { get; set; }
        public string? EmpCode { get; set; }
        public int AC_No { get; set; }
        public int Part_No { get; set; }
        public string? DutyPost { get; set; }

        public int ReasonId { get; set; }
        public string? Remarks { get; set; }

        public string? DocumentPath { get; set; }
        public string? DocumentOriginalName { get; set; }

        public string? ExemptedBy { get; set; }
        public DateTime ExemptionDateTime { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Active"; // Active | Restored
        public string? RestoredBy { get; set; }
        public DateTime? RestoredDateTime { get; set; }
    }
}