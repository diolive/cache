using System.Threading.Tasks;

namespace DioLive.Cache.WebUI.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
