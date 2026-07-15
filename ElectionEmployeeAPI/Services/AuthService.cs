
using Microsoft.EntityFrameworkCore;
using ElectionEmployeeAPI.Data;
using ElectionEmployeeAPI.DTOs;
using ElectionEmployeeAPI.Helpers;
using ElectionEmployeeAPI.Models;

namespace ElectionEmployeeAPI.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly JwtHelper _jwtHelper;
        private readonly EmailService _emailService;

        public AuthService(ApplicationDbContext db, JwtHelper jwtHelper, EmailService emailService)
        {
            _db = db;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
        }
        // ✅ Register
        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return (false, "Passwords do not match");

            if (!IsPasswordValid(dto.Password))
                return (false, "Password does not meet security requirements");

            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId
                    || u.Email == dto.Email
                    || u.MobileNumber == dto.MobileNumber);

            if (existingUser != null)
                return (false, "User ID, Email or Mobile already exists");

            var user = new User
            {
                UserId = dto.UserId,
                FullName = dto.FullName,
                MobileNumber = dto.MobileNumber,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IsActive = false,
                IsApproved = false,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                PasswordChangedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);

            var approval = new AdminApproval
            {
                UserId = dto.UserId,
                RequestType = "Register",
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _db.AdminApprovals.Add(approval);
            await _db.SaveChangesAsync();

            return (true, "Registration successful! Admin approval pending (12-24 hours)");
        }

        // ✅ Login Without OTP
        public async Task<(bool Success, string Message, string? Token, string? Role)>
            LoginWithoutOtpAsync(LoginDto dto)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (user == null)
                return (false, "Invalid User ID or Password", null, null);

            if (!user.IsApproved)
                return (false, "Your account is pending admin approval", null, null);

            if (!user.IsActive)
                return (false, "Your account is inactive", null, null);

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return (false, "Invalid User ID or Password", null, null);

            if (user.IsPasswordExpired)
                return (false, "Your password has expired. Please activate your account", null, null);

            var adminDistrict = await _db.AdminDistricts
       .Where(ad => ad.UserId == user.UserId)
       .Select(ad => ad.District_ID)
       .FirstOrDefaultAsync();

            var token = _jwtHelper.GenerateToken(user, adminDistrict);
            return (true, "Login successful", token, user.Role);
        }

        // ✅ Forgot Password Request
        public async Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (user == null)
                return (false, "User ID not found");

            if (dto.ContactType == "Mobile" && user.MobileNumber != dto.Contact)
                return (false, "Mobile number does not match our records");

            if (dto.ContactType == "Email" && user.Email != dto.Contact)
                return (false, "Email does not match our records");

            var request = new PasswordResetRequest
            {
                UserId = dto.UserId,
                Contact = dto.Contact,
                ContactType = dto.ContactType,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _db.PasswordResetRequests.Add(request);

            var approval = new AdminApproval
            {
                UserId = dto.UserId,
                RequestType = "ForgotPassword",
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _db.AdminApprovals.Add(approval);
            await _db.SaveChangesAsync();

            return (true, "Request sent to admin. Password will be reset within 1-2 hours");
        }

        // ✅ Activate User
        public async Task<(bool Success, string Message)> ActivateUserAsync(ActivateUserDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return (false, "Passwords do not match");

            if (!IsPasswordValid(dto.NewPassword))
                return (false, "Password does not meet security requirements");

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (user == null)
                return (false, "User ID not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
                return (false, "Old password is incorrect");

            var approval = new AdminApproval
            {
                UserId = dto.UserId,
                RequestType = "ActivateUser",
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _db.AdminApprovals.Add(approval);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.PasswordChangedAt = DateTime.UtcNow;
            user.IsActive = false;

            await _db.SaveChangesAsync();

            return (true, "Request sent to admin. Approval within 1-2 hours");
        }

        // ✅ Get User By ID
        public async Task<(string Token, string Role)?> GetUserByIdAsync(string userId)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return null;

            var adminDistrict = await _db.AdminDistricts
      .Where(ad => ad.UserId == user.UserId)
      .Select(ad => ad.District_ID)
      .FirstOrDefaultAsync();

            var token = _jwtHelper.GenerateToken(user, adminDistrict);
            return (token, user.Role);
        }

        // ================= OTP RESET FLOW =================

        // ✅ Send Reset OTP
        public async Task<(bool Success, string Message, string? OtpCode)>
            SendResetOtpAsync(SendResetOtpDto dto)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (user == null)
                return (false, "User ID not found", null);

            if (dto.ContactType == "Mobile" && user.MobileNumber != dto.Contact)
                return (false, "Mobile number does not match our records", null);

            if (dto.ContactType == "Email" && user.Email != dto.Contact)
                return (false, "Email does not match our records", null);

            var oldOtps = await _db.OTPs
                .Where(o => o.UserId == dto.UserId && !o.IsUsed)
                .ToListAsync();

            foreach (var oldOtp in oldOtps)
                oldOtp.IsUsed = true;

            var random = new Random();
            var otpCode = random.Next(100000, 999999).ToString();

            var otp = new OTP
            {
                UserId = dto.UserId,
                OtpCode = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddSeconds(30),
                IsUsed = false
            };

            _db.OTPs.Add(otp);
            await _db.SaveChangesAsync();

            await _emailService.SendOtpEmailAsync(user.Email, otpCode, user.FullName);

            return (true, "OTP sent to your registered email", otpCode);
        }

        // ✅ Verify Reset OTP
        public async Task<(bool Success, string Message)>
            VerifyResetOtpAsync(VerifyResetOtpDto dto)
        {
            var otp = await _db.OTPs
                .Where(o => o.UserId == dto.UserId
                    && o.OtpCode == dto.OtpCode
                    && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null)
                return (false, "Invalid OTP");

            if (otp.IsExpired)
            {
                otp.IsUsed = true;
                await _db.SaveChangesAsync();
                return (false, "OTP has expired. Please request a new one");
            }

            otp.IsUsed = true;
            await _db.SaveChangesAsync();

            return (true, "OTP verified successfully");
        }

        // ✅ Reset Password
        public async Task<(bool Success, string Message)>
            ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return (false, "Passwords do not match");

            if (!IsPasswordValid(dto.NewPassword))
                return (false, "Password does not meet security requirements");

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (user == null)
                return (false, "User ID not found");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.PasswordChangedAt = DateTime.UtcNow;
            user.IsActive = true;

            await _db.SaveChangesAsync();

            return (true, "Password reset successfully! You can now login.");
        }

        public async Task<object?> GetMyApprovalStatusAsync(string userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return null;

            var latestApproval = await _db.AdminApprovals
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.RequestedAt)
                .FirstOrDefaultAsync();

            if (user.IsApproved && user.IsActive)
            {
                return new
                {
                    status = "Approved",
                    remarks = latestApproval?.Remarks ?? "",
                    requestedAt = latestApproval?.RequestedAt ?? user.CreatedAt,
                    approvedAt = latestApproval?.ApprovedAt
                };
            }

            return new
            {
                status = "Pending",
                remarks = latestApproval?.Remarks ?? "",
                requestedAt = latestApproval?.RequestedAt ?? user.CreatedAt,
                approvedAt = latestApproval?.ApprovedAt
            };
        }

        // ✅ Get My Profile (full details, no password)
        public async Task<object?> GetUserProfileAsync(string userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return null;

            return new
            {
                userId = user.UserId,
                fullName = user.FullName,
                mobileNumber = user.MobileNumber,
                email = user.Email,
                role = user.Role,
                isActive = user.IsActive,
                isApproved = user.IsApproved,
                createdAt = user.CreatedAt
            };
        }

        // ✅ Update My Profile (mobile + email only)
        public async Task<(bool Success, string Message)> UpdateUserProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return (false, "User not found");

            var duplicateCheck = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId != userId &&
                    (u.Email == dto.Email || u.MobileNumber == dto.MobileNumber));

            if (duplicateCheck != null)
                return (false, "This mobile number or email is already used by another account");

            user.MobileNumber = dto.MobileNumber;
            user.Email = dto.Email;

            await _db.SaveChangesAsync();

            return (true, "Profile updated successfully");
        }

        // ✅ Password Validation
        private bool IsPasswordValid(string password)
        {
            if (password.Length < 8 || password.Length > 10) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;
            if (!password.Any(c => "!@#$%^&*".Contains(c))) return false;
            return true;
        }
    }
}