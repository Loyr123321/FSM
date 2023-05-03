using System.Configuration;
using System.Net.Mail;

namespace BlazorApp1.Services
{
    public class MailService
    {
        private readonly string ADDRESS;
        private readonly string DISPLAY_NAME;
        private readonly string HOST;
        private readonly int PORT;
        private readonly string USER;
        private readonly string PASSWORD;
        public MailService()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            ADDRESS = configuration.GetValue<string>("MailService:ADDRESS");
            DISPLAY_NAME = configuration.GetValue<string>("MailService:DISPLAY_NAME");
            HOST = configuration.GetValue<string>("MailService:HOST");
            PORT = configuration.GetValue<int>("MailService:PORT");
            USER = configuration.GetValue<string>("MailService:USER");
            PASSWORD = configuration.GetValue<string>("MailService:PASSWORD");
        }
        public async Task SendMail(string To, string Subject, string Body)
        {
            await Task.Run(() =>
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(To);
                mail.From = new MailAddress(ADDRESS, DISPLAY_NAME);
                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient
                {
                    EnableSsl = true,
                    Host = HOST,
                    Port = PORT,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(USER, PASSWORD)
                };

                smtp.Send(mail);
            });

        }

    }
}
