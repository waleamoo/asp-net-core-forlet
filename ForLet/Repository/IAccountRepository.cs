using ForLet.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ForLet.Repository
{
    public interface IAccountRepository
    {
        Task GenerateForgotPasswordTokenAsync(AppUser user);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model);
        Task<AppUser> GetUserByEmailAsync(string email);
    }
}