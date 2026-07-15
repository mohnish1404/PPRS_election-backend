using System;

namespace ElectionEmployeeAPI.DTOs
{
    public class ExemptionListItemDto
    {
        public int ExemptionId { get; set; }
        public string? EmpCode { get; set; }
        public string? EmpName { get; set; }
        public int AC_No { get; set; }
        public int Part_No { get; set; }
        public string? DutyPost { get; set; }
        public string? ReasonText { get; set; }
        public string? Remarks { get; set; }
        public string? DocumentPath { get; set; }
        public string? ExemptedBy { get; set; }
        public DateTime ExemptionDateTime { get; set; }
        public string? Status { get; set; }
        public string? RestoredBy { get; set; }
        public DateTime? RestoredDateTime { get; set; }
    }
}