using MailKit.Net.Smtp;
using MimeKit;

namespace GqeberhaPharmacy.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string message)
        {
            try
            {
                var smtpServer = _configuration["Email:SmtpServer"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var fromEmail = _configuration["Email:FromEmail"];
                var fromPassword = _configuration["Email:FromPassword"];

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress("Ibhayi Pharmacy", fromEmail));
                mimeMessage.To.Add(MailboxAddress.Parse(to));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart("html") { Text = message };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(fromEmail, fromPassword);
                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Email sent to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
            }
        }

        public async Task SendPrescriptionReadyNotificationAsync(string customerEmail, string customerName, string prescriptionId)
        {
            string subject = "Your Prescription is Ready - Ibhayi Pharmacy";
            string message = $@"
                <h2>Hello {customerName},</h2>
                <p>Your prescription (ID: {prescriptionId}) is now ready for collection at Ibhayi Pharmacy.</p>
                <p>Please come to collect your medication at your earliest convenience.</p>
                <p>Best regards,<br>Ibhayi Pharmacy Team</p>
            ";
            await SendEmailAsync(customerEmail, subject, message);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            string subject = "Password Reset - Ibhayi Pharmacy";
            string message = $@"
                <p>Please click the link below to reset your password:</p>
                <a href='{resetLink}'>Reset Password</a>
                <p>If you did not request a password reset, please ignore this email.</p>
            ";
            await SendEmailAsync(email, subject, message);
        }

        public async Task SendStockOrderEmailAsync(string supplierEmail, string supplierName, string orderNumber, string medicationList)
        {
            string subject = $"Stock Order #{orderNumber} - Ibhayi Pharmacy";
            string message = $@"
                <h2>Hello {supplierName},</h2>
                <p>Please find below the stock order details from Ibhayi Pharmacy.</p>
                <p><strong>Order Number: {orderNumber}</strong></p>
                <h3>Medications Ordered:</h3>
                {medicationList}
                <p>Please confirm receipt of this order.</p>
                <p>Best regards,<br>Ibhayi Pharmacy</p>
            ";
            await SendEmailAsync(supplierEmail, subject, message);
        }
    }
}
