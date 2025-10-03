using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

namespace vocafind_api.Services
{
    public class EmailSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; }
        public string From { get; set; } = "";
        public string User { get; set; } = "";
        public string Pass { get; set; } = "";
    }


    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Vocafind", _settings.From));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            // wrap body ke template HTML
            var htmlBody = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px; }}
                    .container {{ max-width: 600px; margin: auto; background: #ffffff; border-radius: 8px; padding: 20px; 
                                  box-shadow: 0 2px 8px rgba(0,0,0,0.1); }}
                    h2 {{ color: #2c3e50; }}
                    p {{ font-size: 14px; color: #333; }}
                    .footer {{ margin-top: 20px; font-size: 12px; color: #888; text-align: center; }}
                    .btn {{
                        display: inline-block;
                        padding: 10px 20px;
                        margin-top: 15px;
                        font-size: 14px;
                        color: #fff;
                        background-color: #3498db;
                        text-decoration: none;
                        border-radius: 5px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2>{subject}</h2>
                    <p>Halo <b>{toEmail}</b>,</p>
                    <p>{body}</p>
                    <a href='https://vocafind.com/login' class='btn'>Login Sekarang</a>
                    <div class='footer'>
                        <p>&copy; {DateTime.Now.Year} Vocafind. Semua Hak Dilindungi.</p>
                    </div>
                </div>
            </body>
            </html>";

            email.Body = new TextPart("html") { Text = htmlBody };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.User, _settings.Pass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

    }
}
