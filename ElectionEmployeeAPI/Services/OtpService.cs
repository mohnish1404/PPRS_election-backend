using ElectionEmployeeAPI.Models;
using Microsoft.EntityFrameworkCore;
using ElectionEmployeeAPI.Data;

namespace ElectionEmployeeAPI.Services
{
    public class OtpService
    {
        private readonly ApplicationDbContext _db;

        public OtpService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ✅ OTP Generate karo
        public async Task<(bool Success, string Message, string? OtpCode)>
            GenerateOtpAsync(string userId, string mobileNumber)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == userId
                    && u.MobileNumber == mobileNumber);

            if (user == null)
                return (false, "User ID or Mobile Number is incorrect", null);

            if (!user.IsApproved)
                return (false, "Your account is pending admin approval", null);

            if (!user.IsActive)
                return (false, "Your account is inactive", null);

            // Purane OTPs expire karo
            var oldOtps = await _db.OTPs
                .Where(o => o.UserId == userId && !o.IsUsed)
                .ToListAsync();

            foreach (var oldOtp in oldOtps)
                oldOtp.IsUsed = true;

            // Naya OTP banao
            var otpCode = GenerateRandomOtp();

            var otp = new OTP
            {
                UserId = userId,
                OtpCode = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddSeconds(30),
                IsUsed = false
            };

            _db.OTPs.Add(otp);
            await _db.SaveChangesAsync();

            // Real app mein yahan SMS bhejenge (Twilio/MSG91)
            // Abhi development mein OTP response mein return kar rahe hain
            return (true, "OTP sent successfully", otpCode);
        }

        // ✅ OTP Verify karo
        public async Task<(bool Success, string Message)>
            VerifyOtpAsync(string userId, string otpCode)
        {
            var otp = await _db.OTPs
                .Where(o => o.UserId == userId
                    && o.OtpCode == otpCode
                    && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null)
                return (false, "Invalid OTP");

            if (otp.IsExpired)
            {
                otp.IsUsed = true;
                await _db.SaveChangesAsync();
                return (false, "OTP has expired");
            }

            otp.IsUsed = true;
            await _db.SaveChangesAsync();

            return (true, "OTP verified successfully");
        }

        // ✅ Random 6 digit OTP
        private string GenerateRandomOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}