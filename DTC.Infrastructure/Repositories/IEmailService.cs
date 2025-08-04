namespace DTC.Infrastructure.Repositories
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}
