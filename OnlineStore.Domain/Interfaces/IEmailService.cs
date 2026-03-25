using System.Threading.Tasks;

namespace OnlineStore.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendOrderConfirmationAsync(string userEmail, string orderId, decimal totalAmount);
    }
}
