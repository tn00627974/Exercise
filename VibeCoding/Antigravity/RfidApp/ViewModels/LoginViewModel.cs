using System.Windows.Input;
using RfidApp.Services;

namespace RfidApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private string _username;
        private string _password;
        private string _errorMessage;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(OnLogin);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        private async void OnLogin()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;

            var user = await _authService.LoginAsync(Username, Password);
            
            if (user != null)
            {
                // Navigate to Main Shell
                await Shell.Current.GoToAsync("//Main/DashboardPage");
            }
            else
            {
                ErrorMessage = "Invalid Username or Password";
            }

            IsBusy = false;
        }
    }
}
