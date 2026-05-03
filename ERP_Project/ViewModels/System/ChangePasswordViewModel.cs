using ERP_Project.Commands;
using ERP_Project.Services.Navigation;
using ERP_Project.Services.Users;
using ERP_Project.Stores;
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

        public bool IsForceChange { get; set; } = true;        // 로그인 직후 true

        private string _currentPassword = string.Empty;
        public string CurrentPassword
        {
            get => _currentPassword;
            set
            {
                _currentPassword = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanChangePassword));
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
                OnPropertyChanged(nameof(CanChangePassword));
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
                OnPropertyChanged(nameof(CanChangePassword));
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

        public bool CanChangePassword => !string.IsNullOrWhiteSpace(CurrentPassword)
                                       && !string.IsNullOrWhiteSpace(NewPassword)
                                       && !string.IsNullOrWhiteSpace(ConfirmPassword);
        
        public ICommand ChangePasswordCommand { get; }
        public ICommand CancelCommand { get; }

        public ChangePasswordViewModel(IUserService userService, UserSessionStore userSessionStore, INavigationService navigationService)
        {
            _userService = userService;
            _userSessionStore = userSessionStore;
            _navigationService = navigationService;

            ChangePasswordCommand = new RelayCommand<object>(async _ => await ChangePassword());
            CancelCommand = new RelayCommand<object>(_ => Cancel());
        }

        private async Task ChangePassword()
        {
            StatusMessage = string.Empty;

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

                switch (result)
                {
                    case "SUCCESS":
                        StatusMessage = "비밀번호가 성공적으로 변경되었습니다.";

                        await Task.Delay(800);
                        _navigationService.Navigate(NaviType.CurrentStockView);
                        break;

                    case "INVALID_PASSWORD":
                        StatusMessage = "현재 비밀번호가 올바르지 않습니다.";
                        break;

                    case "SAME_PASSWORD":
                        StatusMessage = "기존 비밀번호와 동일한 비밀번호는 사용할 수 없습니다.";
                        break;

                    case "NOT_FOUND":
                        StatusMessage = "사용자를 찾을 수 없습니다.";
                        break;

                    default:
                        StatusMessage = "비밀번호 변경에 실패했습니다.";
                        break;
                }

            }
            catch (Exception ex)
            {
                StatusMessage = $"오류 발생: {ex.Message}";
            }
        }

        private void Cancel()
        {
            _navigationService.Navigate(NaviType.CurrentStockView);
        }
    }
}