namespace ElectionEmployeeAPI.DTOs
{
    public class RemoveDutyRequest
    {
        public List<int> MemberIds { get; set; } = new();
        public string? Remarks { get; set; }
    }
}
