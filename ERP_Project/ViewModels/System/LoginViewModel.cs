using ERP_Project.Commands;
using ERP_Project.Services.Navigation;
using ERP_Project.Services.Users;
using ERP_Project.Stores;
using ERP_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERP_Project.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly UserSessionStore _userSessionStore;
        private readonly INavigationService _navigationService;
        private readonly IUserService _userService;

        private string _userId = string.Empty;

        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        private string _password = string.Empty;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage = string.Empty;

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand LoginCommand { get; }
        public ICommand NavigateChangePasswordCommand { get; }

        public LoginViewModel(UserSessionStore userSessionStore, INavigationService navigationService, IUserService userService)
        {
            _userSessionStore = userSessionStore;
            _navigationService = navigationService;
            _userService = userService;

            LoginCommand = new RelayCommand<object>(async _ => await Login());
            NavigateChangePasswordCommand = new RelayCommand<object>(_ => _navigationService.Navigate(NaviType.ChangePasswordView));
        }

        private async Task Login()
        {
            StatusMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(Password))
            {
                StatusMessage = "아이디와 비밀번호를 입력해주세요.";
                return;
            }

            try
            {
                var user = await _userService.LoginAsync(UserId, Password);

                if (user != null)
                {
                    // 로그인 성공
                    _userSessionStore.SetUser(user);
                    _userSessionStore.Login();

                    // 임시 비밀번호 강제 변경
                    if (Password == "0000")
                    {
                        _navigationService.Navigate(NaviType.ChangePasswordView);
                        return;
                    }

                    // 정상 로그인
                    _navigationService.Navigate(NaviType.CurrentStockView);
                }
                else
                {
                    StatusMessage = "아이디 또는 비밀번호가 올바르지 않습니다.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"로그인 중 오류가 발생했습니다: {ex.Message}";
            }
        }
    }
}