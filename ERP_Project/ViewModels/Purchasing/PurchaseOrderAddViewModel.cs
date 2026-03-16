using ERP_Project.Commands;
using ERP_Project.Models.Master;
using ERP_Project.Models.Purchasing;
using ERP_Project.Services.Customers;
using ERP_Project.Services.Items;
using ERP_Project.Services.Navigation;
using ERP_Project.Services.PurchaseOrders;
using ERP_Project.Services.Warehouses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ERP_Project.ViewModels
{
    public class PurchaseOrderAddViewModel : ViewModelBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ICustomerService _customerService;
        private readonly IItemService _itemService;
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

        private ObservableCollection<Item> _itemsLookup;

        public ObservableCollection<Item> ItemsLookup
        {
            get { return _itemsLookup; }
            set
            {
                _itemsLookup = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Warehouse> _warehouseLookup;

        public ObservableCollection<Warehouse> WarehousesLookup
        {
            get { return _warehouseLookup; }
            set
            {
                _warehouseLookup = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> StatusList { get; set; }

        // Master
        private PurchaseOrderM _master;

        public PurchaseOrderM Master
        {
            get { return _master; }
            set { 
                _master = value;
                OnPropertyChanged();
            }
        }

        // Detail
        public ObservableCollection<PurchaseOrderD> Details { get; set; }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public PurchaseOrderAddViewModel(IPurchaseOrderService purchaseOrderService, ICustomerService customerService, IItemService itemService, IWarehouseService warehouseService, INavigationService navigationService)
        {
            _purchaseOrderService = purchaseOrderService;
            _customerService = customerService;
            _itemService = itemService;
            _warehouseService = warehouseService;
            _navigationService = navigationService;

            SuppliersLookup = new ObservableCollection<Customer>();
            ItemsLookup = new ObservableCollection<Item>();
            WarehousesLookup = new ObservableCollection<Warehouse>();
            StatusList = new ObservableCollection<string> { "0-임시", "1-확정", };

            Master = new PurchaseOrderM
            {
                OrderDate = DateTime.Today,
                Status = 0,
                CreatedBy = 1
            };
            Details = new ObservableCollection<PurchaseOrderD>();
            Details.CollectionChanged += Details_CollectionChanged;

            SaveCommand = new RelayCommand<object>(async _ => await Save(), _ => true);
            CancelCommand = new RelayCommand<object>(Cancel);

            _ = LoadLookupData();
        }

        // Methods
        private void Details_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;

            foreach (PurchaseOrderD d in e.NewItems)
            {
                d.PropertyChanged += Detail_PropertyChanged;
            }
        }

        private void Detail_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PurchaseOrderD.ItemId))
            {
                var detail = sender as PurchaseOrderD;

                var item = ItemsLookup.FirstOrDefault(x => x.ItemId == detail.ItemId);

                if (item != null)
                {
                    detail.ItemName = item.ItemName;
                    detail.UnitPrice = item.StandardCost;
                }
            }
        }

        private async Task LoadLookupData()
        {
            // Supplier Lookup
            var suppliers = await _customerService.GetLookupAsync();
            SuppliersLookup = new ObservableCollection<Customer>(suppliers);

            // Item Lookup
            var items = await _itemService.GetLookupAsync();
            ItemsLookup = new ObservableCollection<Item>(items);

            // Warehouse Lookup
            var warehouses = await _warehouseService.GetLookupAsync();
            WarehousesLookup = new ObservableCollection<Warehouse>(warehouses);
        }

        private async Task Save()
        {
            if (Master.SupplierId == 0)
            {
                MessageBox.Show("거래처를 선택하세요.");
                return;
            }

            if (Master.WarehouseId == 0)
            {
                MessageBox.Show("창고를 선택하세요.");
                return;
            }

            if (Details.Count == 0)
            {
                MessageBox.Show("발주 상세를 추가하세요.");
                return;
            }

            foreach (var d in Details)
            {
                if (d.ItemId == 0)
                {
                    MessageBox.Show("품목을 선택하세요.");
                    return;
                }
                if (d.Qty <= 0)
                {
                    MessageBox.Show("수량은 0보다 커야합니다.");
                    return;
                }

                d.CreatedBy = Master.CreatedBy;
            }

            if (Master.Status == 1) // 확정
            {
                // 확정시 BOM 생성, Inventory 반영 가능
            }

            var validDetails = Details.Where(d => d.ItemId != 0).ToList();

            var result = await _purchaseOrderService.SaveAsync(Master, validDetails);

            if (result.Code == "MSG0001")
            {
                MessageBox.Show("발주가 등록되었습니다.");
                _navigationService.Navigate(NaviType.PurchaseOrderView);
            }
            else
            {
                MessageBox.Show(result.Message);
            }
        }

        private void Cancel(object _)
        {
            _navigationService.Navigate(NaviType.PurchaseOrderView);
        }
    }
}