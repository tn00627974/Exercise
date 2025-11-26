using System.Threading.Tasks;
using RfidApp.Models;

namespace RfidApp.Services
{
    public interface IAuthService
    {
        Task<User> LoginAsync(string username, string password);
        Task LogoutAsync();
        User GetCurrentUser();
        bool IsAuthenticated();
    }
}
