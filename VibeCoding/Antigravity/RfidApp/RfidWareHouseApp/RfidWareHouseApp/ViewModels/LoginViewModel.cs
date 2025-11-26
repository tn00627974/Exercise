using System.Windows.Input;
using RfidApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls; // Application

namespace RfidApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private string _username;
        private string _password;
        private string _errorMessage;

        // 公用無參數建構子，供 XAML 建構使用
        //public LoginViewModel()
        //    : this(Application.Current?.Services?.GetService<IAuthService>() ?? throw new InvalidOperationException("IAuthService not registered"))
        //{
        //}

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
