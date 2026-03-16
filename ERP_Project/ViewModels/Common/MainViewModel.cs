using ERP_Project.Commands;
using ERP_Project.Services.Navigation;
using ERP_Project.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERP_Project.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly MainNavigationStore _mainNavigationStore;
        private readonly INavigationService _navigationService;
        private readonly UserSessionStore _userSessionStore;

        public ViewModelBase? CurrentViewModel => _mainNavigationStore.CurrentViewModel;

        public bool IsLoggedIn => _userSessionStore.IsLoggedIn;

        // Commands
        public ICommand ShowItemCommand { get; }
        public ICommand ShowPurchaseCommand { get; }
        public ICommand ShowProductionCommand { get; }
        public ICommand ShowSalesCommand { get; }
        public ICommand ShowInventoryCommand { get; }
        public ICommand ShowCurrentStockCommand { get; }

        public MainViewModel(MainNavigationStore mainNavigationStore, INavigationService navigationService, UserSessionStore userSessionStore)
        {
            _mainNavigationStore = mainNavigationStore;
            _navigationService = navigationService;
            _userSessionStore = userSessionStore;

            // NavigationStore 변경 시 CurrentViewModel 갱신
            _mainNavigationStore.CurrentViewModelChanged += () =>
            {
                OnPropertyChanged(nameof(CurrentViewModel));
            };

            // 로그인 상태 변경 시 IsLoggedIn 갱신
            _userSessionStore.LoginStateChanged += () =>
            {
                OnPropertyChanged(nameof(IsLoggedIn));
            };

            // TopBar 버튼 Command
            ShowItemCommand = new RelayCommand<object>(_ => _navigationService.Navigate(NaviType.ItemView));
            ShowPurchaseCommand = new RelayCommand<object>(_ => _navigationService.Navigate(NaviType.PurchaseOrderView));
            ShowProductionCommand = new RelayCommand<object>(_ => _navigationService.Navigate(NaviType.ProductionOrderView));
            ShowSalesCommand = new RelayCommand<object>(_ => _navigationService.Navigate(NaviType.SalesOrderView));
            ShowInventoryCommand = new RelayCommand<object>(_ => _navigationService.Navigate(NaviType.InventoryView));
            ShowCurrentStockCommand = new RelayCommand<object>(_ => _navigationService.Navigate(NaviType.CurrentStockView));
        }
    }
}