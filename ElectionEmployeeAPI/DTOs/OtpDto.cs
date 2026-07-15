namespace ElectionEmployeeAPI.DTOs
{
    public class OtpDto
    {
        public string UserId { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string? OtpCode { get; set; }
    }

    public class ActivateUserDto
    {
        public string UserId { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string ContactType { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string UserId { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class SendResetOtpDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string ContactType { get; set; } = string.Empty;
    }

    public class VerifyResetOtpDto
    {
        public string UserId { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }
}