using ERP_Project.Stores;
using ERP_Project.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly MainNavigationStore _mainNavigationStore;
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(MainNavigationStore mainNavigationStore, IServiceProvider serviceProvider)
        {
            _mainNavigationStore = mainNavigationStore;
            _serviceProvider = serviceProvider;
        }

        public void Navigate(NaviType naviType, object? parameter = null)
        {
            ViewModelBase viewModel = naviType switch
            {
                NaviType.LoginView =>
                    _serviceProvider.GetRequiredService<LoginViewModel>(),
                NaviType.ItemView =>
                    _serviceProvider.GetRequiredService<ItemViewModel>(),
                NaviType.CustomerView =>
                    _serviceProvider.GetRequiredService<CustomerViewModel>(),
                NaviType.WarehouseView =>
                    _serviceProvider.GetRequiredService<WarehouseViewModel>(),
                NaviType.BOMView =>
                    _serviceProvider.GetRequiredService<BOMViewModel>(),
                NaviType.PurchaseOrderView =>
                    _serviceProvider.GetRequiredService<PurchaseOrderViewModel>(),
                NaviType.SalesOrderView =>
                    _serviceProvider.GetRequiredService<SalesOrderViewModel>(),
                NaviType.ProductionOrderView =>
                    _serviceProvider.GetRequiredService<ProductionOrderViewModel>(),
                NaviType.DashboardView =>
                    _serviceProvider.GetRequiredService<DashboardViewModel>(),
                NaviType.ChangePasswordView =>
                    _serviceProvider.GetRequiredService<ChangePasswordViewModel>(),
                NaviType.InventoryView =>
                    _serviceProvider.GetRequiredService<InventoryViewModel>(),
                NaviType.CurrentStockView =>
                    _serviceProvider.GetRequiredService<CurrentStockViewModel>(),
                NaviType.InventoryAddView =>
                    _serviceProvider.GetRequiredService<InventoryAddViewModel>(),
                NaviType.PurchaseOrderAddView =>
                    _serviceProvider.GetRequiredService<PurchaseOrderAddViewModel>(),
                NaviType.PurchaseOrderDetailView =>
                    _serviceProvider.GetRequiredService<PurchaseOrderDetailViewModel>(),
                   
                _ => throw new ArgumentOutOfRangeException()
            };

            // Detail 화면이면 parameter (MasterId) 전달
            if (viewModel is PurchaseOrderDetailViewModel detailVm && parameter is int masterId)
            {
                _ = detailVm.LoadDetailAsync(masterId);
            }

            _mainNavigationStore.CurrentViewModel = viewModel;
        }
    }
}