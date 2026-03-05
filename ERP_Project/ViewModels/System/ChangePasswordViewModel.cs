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
    public class ChangePasswordViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly UserSessionStore _userSessionStore;
        private readonly INavigationService _navigationService;

        private string _currentPassword = string.Empty;
        public string CurrentPassword
        {
            get => _currentPassword;
            set
            {
                _currentPassword = value;
                OnPropertyChanged();
            }
        }

        private string _newPassword = string.Empty;
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged();
            }
        }

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChangePasswordCommand { get; }

        public ChangePasswordViewModel(
            IUserService userService,
            UserSessionStore userSessionStore,
            INavigationService navigationService)
        {
            _userService = userService;
            _userSessionStore = userSessionStore;
            _navigationService = navigationService;

            ChangePasswordCommand = new RelayCommand<object>(async _ => await ChangePassword());
        }

        private async Task ChangePassword()
        {
            StatusMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                StatusMessage = "모든 항목을 입력해주세요.";
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                StatusMessage = "새 비밀번호가 일치하지 않습니다.";
                return;
            }

            if (NewPassword.Length < 4)
            {
                StatusMessage = "비밀번호는 최소 4자리 이상이어야 합니다.";
                return;
            }

            try
            {
                var userId = _userSessionStore.CurrentUser?.UserId;

                if (userId == null)
                {
                    StatusMessage = "사용자 정보가 없습니다. 다시 로그인해주세요.";
                    return;
                }

                var result = await _userService.ChangePasswordAsync(
                    userId.Value,
                    CurrentPassword,
                    NewPassword);

                if (result)
                {
                    StatusMessage = "비밀번호가 성공적으로 변경되었습니다.";

                    // 변경 후 대시보드 이동
                    await Task.Delay(1000);
                    _navigationService.Navigate(NaviType.DashboardView);
                }
                else
                {
                    StatusMessage = "현재 비밀번호가 올바르지 않습니다.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"비밀번호 변경 중 오류 발생: {ex.Message}";
            }
        }
    }
}