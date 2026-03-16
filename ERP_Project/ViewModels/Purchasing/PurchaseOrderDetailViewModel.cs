using ERP_Project.Commands;
using ERP_Project.Models.Master;
using ERP_Project.Models.Purchasing;
using ERP_Project.Services.Customers;
using ERP_Project.Services.Navigation;
using ERP_Project.Services.PurchaseOrders;
using ERP_Project.Services.Warehouses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ERP_Project.ViewModels
{
    public class PurchaseOrderDetailViewModel : ViewModelBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ICustomerService _customerService;
        private readonly IWarehouseService _warehouseService;
        private readonly INavigationService _navigationService;

        // Lookup
        private ObservableCollection<Customer> _suppliersLookup;

        public ObservableCollection<Customer> SuppliersLookup
        {
            get { return _suppliersLookup; }
            set { 
                _suppliersLookup = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Warehouse> _warehousesLookup;

        public ObservableCollection<Warehouse> WarehousesLookup
        {
            get { return _warehousesLookup; }
            set
            {
                _warehousesLookup = value;
                OnPropertyChanged();
            }
        }

        public PurchaseOrderM SelectedPurchaseOrder { get; set; }

        public ObservableCollection<PurchaseOrderD> Details { get; set; }

        // Commands
        public ICommand ConfirmCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CloseCommand { get; }
        
        public PurchaseOrderDetailViewModel(IPurchaseOrderService purchaseOrderService, ICustomerService customerService, IWarehouseService warehouseService, INavigationService navigationService)
        {
            _purchaseOrderService = purchaseOrderService;
            _customerService = customerService;
            _warehouseService = warehouseService;
            _navigationService = navigationService;

            SuppliersLookup = new ObservableCollection<Customer>();
            WarehousesLookup = new ObservableCollection<Warehouse>();
            Details = new ObservableCollection<PurchaseOrderD>();

            ConfirmCommand = new RelayCommand<object>(Confirm, _ => CanConfirm);
            SaveCommand = new RelayCommand<object>(Save, _ => CanSave);
            CloseCommand = new RelayCommand<object>(Close);

            _ = LoadLookupData();
        }

        // Methods
        private async Task LoadLookupData()
        {
            // Supplier Lookup
            var suppliers = await _customerService.GetLookupAsync();
            SuppliersLookup = new ObservableCollection<Customer>(suppliers);

            // Supplier Lookup
            var warehouses = await _warehouseService.GetLookupAsync();
            WarehousesLookup = new ObservableCollection<Warehouse>(warehouses);
        }

        public async Task LoadDetailAsync(int purchaseOrderMId)
        {
            SelectedPurchaseOrder = await _purchaseOrderService.GetMasterAsync(purchaseOrderMId);

            var details = await _purchaseOrderService.GetDetailsAsync(purchaseOrderMId);

            Details.Clear();
            foreach (var d in details)
                Details.Add(d);

            OnPropertyChanged(nameof(SelectedPurchaseOrder));
        }

        private async void Confirm(object _)
        {
            if (SelectedPurchaseOrder == null)
                return;

            // 이미 확정된 발주 방지
            if (SelectedPurchaseOrder.Status != 0)
            {
                MessageBox.Show("이미 확정된 발주입니다.");
                return;
            }

            var result = MessageBox.Show("발주를 확정하시겠습니까?\n확정 시 재고가 입고 처리됩니다.", "발주 확정", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var serviceResult = await _purchaseOrderService.ConfirmPurchaseOrderAsync(SelectedPurchaseOrder.PurchaseOrderMId);

                if (serviceResult.Code.StartsWith("MSG"))
                {
                    MessageBox.Show("발주가 정상적으로 확정되었습니다.", "확정 완료");

                    // 상태 갱신
                    SelectedPurchaseOrder.Status = 1;
                    OnPropertyChanged(nameof(SelectedPurchaseOrder));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "시스템 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanConfirm => SelectedPurchaseOrder?.Status == 0;
       
        private async void Save(object _)
        {
            if (SelectedPurchaseOrder == null)
                return;

            // 이미 확정된 발주면 저장 불가
            if (SelectedPurchaseOrder.Status != 0)
            {
                MessageBox.Show("확정된 발주는 수정할 수 없습니다.");
                return;
            }

            try
            {
                // Master 업데이트
                var masterResult = await _purchaseOrderService.UpdateMasterAsync(SelectedPurchaseOrder);

                if (!masterResult.Code.StartsWith("MSG"))
                {
                    MessageBox.Show($"마스터 저장 실패: {masterResult.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Detail 업데이트
                foreach (var detail in Details)
                {
                    var detailResult = await _purchaseOrderService.UpdateDetailAsync(detail);
                    if (!detailResult.Code.StartsWith("MSG"))
                    {
                        MessageBox.Show($"상세 저장 실패 (Item: {detail.ItemCode}): {detailResult.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                MessageBox.Show("발주가 정상적으로 저장되었습니다.", "저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "시스템 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanSave => SelectedPurchaseOrder?.Status == 0;

        private void Close(object _)
        {
            _navigationService.Navigate(NaviType.PurchaseOrderView);
        }
    }
}