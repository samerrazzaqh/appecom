using Core.DTO;
using Core.Services;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Infrastructure.Repositries.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        public async Task SendEmail(EmailDTO emailDTO)
        {
            MimeMessage message = new();

            message.From.Add(new MailboxAddress("My Ecom", configuration["EmailSetting:From"]));
            message.Subject = emailDTO.Subject;
            message.To.Add(new MailboxAddress(emailDTO.To, emailDTO.To));
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailDTO.Content
            };

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    int port = int.Parse(configuration["EmailSetting:Port"]!);

                    // 1. تحديد نوع التشفير بدقة بحسب المنفذ
                    SecureSocketOptions socketOptions = port switch
                    {
                        465 => SecureSocketOptions.SslOnConnect,
                        587 => SecureSocketOptions.StartTls,
                        25 => SecureSocketOptions.None,
                        _ => SecureSocketOptions.Auto
                    };

                    // 2. لتجاوز أخطاء شهادات SSL في البيئة المحلية (اختياري)
                    smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await smtp.ConnectAsync(
                        configuration["EmailSetting:Smtp"],
                        port,
                        socketOptions);

                    await smtp.AuthenticateAsync(
                        configuration["EmailSetting:Username"],
                        configuration["EmailSetting:Password"]);

                    await smtp.SendAsync(message);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    await smtp.DisconnectAsync(true);
                }
            }
        }
        
        
        
    }
}
