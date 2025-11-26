using System.Threading.Tasks;
using RfidWareHouseApp.Models;

namespace RfidWareHouseApp.Services
{
    public class AuthService : IAuthService
    {
        private User _currentUser;

        public Task<User> LoginAsync(string username, string password)
        {
            // Mock Login Logic
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                _currentUser = new User
                {
                    Id = "1",
                    Username = username,
                    FullName = "Demo User",
                    Role = "Admin"
                };
                return Task.FromResult(_currentUser);
            }
            return Task.FromResult<User>(null);
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            return Task.CompletedTask;
        }

        public User GetCurrentUser()
        {
            return _currentUser;
        }

        public bool IsAuthenticated()
        {
            return _currentUser != null;
        }
    }
}
