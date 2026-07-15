using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ElectionEmployeeAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode, string userName)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                _config["EmailSettings:SenderName"],
                _config["EmailSettings:SenderEmail"]
            ));
            Console.WriteLine($"Sending OTP to: {toEmail}");
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "OTP - Election Portal Password Reset";

            email.Body = new TextPart("html")
            {
                Text = $@"
                <div style='font-family:Arial,sans-serif;max-width:500px;margin:auto;padding:30px;border:1px solid #e2e8f0;border-radius:12px;'>
                    <h2 style='color:#1e3a8a;'>कार्यालय मुख्य निर्वाचन पदाधिकारी</h2>
                    <p>Dear <b>{userName}</b>,</p>
                    <p>Your OTP for password reset is:</p>
                    <div style='font-size:36px;font-weight:bold;letter-spacing:10px;color:#1e3a8a;text-align:center;padding:20px;background:#f0f4ff;border-radius:8px;margin:20px 0;'>
                        {otpCode}
                    </div>
                    <p style='color:#ef4444;'>⚠️ This OTP is valid for 30 seconds only.</p>
                    <p style='color:#64748b;font-size:12px;'>Never share this OTP with anyone.</p>
                    <hr/>
                    <p style='color:#94a3b8;font-size:11px;'>Election Commission — Chhattisgarh</p>
                </div>"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config["EmailSettings:SmtpHost"],
                int.Parse(_config["EmailSettings:SmtpPort"]!),
                SecureSocketOptions.StartTls
            );
            await smtp.AuthenticateAsync(
                _config["EmailSettings:SenderEmail"],
                _config["EmailSettings:SenderPassword"]
            );
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}