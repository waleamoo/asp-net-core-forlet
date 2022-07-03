using ForLet.Models;
using System.Threading.Tasks;

namespace ForLet.Services
{
    public interface IEmailService
    {
        Task SendTestEmail(UserEmailOptions userEmailOptions);
        Task SendEmailForEmailConfirmation(UserEmailOptions options);
        Task SendEmailForForgotPassword(UserEmailOptions userEmailOptions);
    }
}