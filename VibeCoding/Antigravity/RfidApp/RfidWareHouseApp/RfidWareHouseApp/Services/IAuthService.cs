using System.Threading.Tasks;
using RfidWareHouseApp.Models;

namespace RfidWareHouseApp.Services
{
    public interface IAuthService
    {
        Task<User> LoginAsync(string username, string password);
        Task LogoutAsync();
        User GetCurrentUser();
        bool IsAuthenticated();
    }
}
