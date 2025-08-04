using DTC.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DTC.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var smtpSection = _configuration.GetSection("SmtpSettings");
            var from = smtpSection["From"];
            var host = smtpSection["Host"];
            var port = int.Parse(smtpSection["Port"]);
            var username = smtpSection["Username"];
            var password = smtpSection["Password"];
            var enableSsl = bool.Parse(smtpSection["EnableSsl"]);

            using var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            await smtpClient.SendMailAsync(mail);
        }
    }
}
