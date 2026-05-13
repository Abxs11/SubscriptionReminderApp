using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SubscriptionReminder.Api.Services.Interfaces;

namespace SubscriptionReminder.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var emailSettings = _config.GetSection("EmailSettings");
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            // Sertifika hatalarını görmezden gelmek için (Test ortamı için gerekebilir)
            // client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(
                emailSettings["SmtpServer"], 
                int.Parse(emailSettings["SmtpPort"] ?? "587"), 
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(emailSettings["SmtpUser"], emailSettings["SmtpPass"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("{To} adresine e-posta başarıyla gönderildi.", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{To} adresine e-posta gönderilirken bir hata oluştu.", to);
            // Hata fırlatmıyoruz ki background servis durmasın
        }
    }
}
