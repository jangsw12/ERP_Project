using ERP_Project.Commands;
using ERP_Project.Models.Inventory;
using ERP_Project.Models.Master;
using ERP_Project.Services.Inventorys;
using ERP_Project.Services.Items;
using ERP_Project.Services.Navigation;
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
    public class InventoryAddViewModel : ViewModelBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IItemService _itemService;
        private readonly IWarehouseService _warehouseService;
        private readonly INavigationService _navigationService;

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

        public ObservableCollection<string> TranTypes { get; set; }
        public ObservableCollection<string> RefTypes { get; set; }

        // Input Data
        private int _itemId;

        public int ItemId
        {
            get { return _itemId; }
            set
            {
                if (_itemId != value)
                {
                    _itemId = value;
                    OnPropertyChanged();

                    // 선택된 품목의 단가 자동 할당
                    var selectedItem = ItemsLookup.FirstOrDefault(x => x.ItemId == _itemId);
                    if (selectedItem != null)
                        UnitCost = selectedItem.StandardCost;
                    else
                        UnitCost = null;
                }
            }
        }

        private int _warehouseId;

        public int WarehouseId
        {
            get { return _warehouseId; }
            set
            {
                _warehouseId = value;
                OnPropertyChanged();
            }
        }

        private string _tranType;

        public string TranType
        {
            get { return _tranType; }
            set
            {
                _tranType = value;
                OnPropertyChanged();
            }
        }

        private string? _refType;

        public string? RefType
        {
            get { return _refType; }
            set
            {
                _refType = value;
                OnPropertyChanged();
            }
        }

        private string _refNo;

        public string RefNo
        {
            get { return _refNo; }
            set
            {
                _refNo = value;
                OnPropertyChanged();
            }
        }

        private decimal _qty;

        public decimal Qty
        {
            get { return _qty; }
            set
            {
                _qty = value;
                OnPropertyChanged();
            }
        }

        private decimal? _unitCost;

        public decimal? UnitCost
        {
            get { return _unitCost; }
            set
            {
                _unitCost = value;
                OnPropertyChanged();
            }
        }

        private string? _tranRemark;

        public string? TranRemark
        {
            get { return _tranRemark; }
            set
            {
                _tranRemark = value;
                OnPropertyChanged();
            }
        }

        private DateTime _tranDate = DateTime.Today;

        public DateTime TranDate
        {
            get { return _tranDate; }
            set { 
                _tranDate = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public InventoryAddViewModel(IInventoryService inventoryService, IItemService itemService, IWarehouseService warehouseService, INavigationService navigationService)
        {
            _inventoryService = inventoryService;
            _itemService = itemService;
            _warehouseService = warehouseService;
            _navigationService = navigationService;

            ItemsLookup = new ObservableCollection<Item>();
            WarehousesLookup = new ObservableCollection<Warehouse>();

            TranTypes = new ObservableCollection<string> { "IN", "OUT", "PROD_IN", "PROD_OUT" };
            RefTypes = new ObservableCollection<string> { "PO", "SO", "PR" };

            SaveCommand = new RelayCommand<object>(async _ => await Save(), _ => true);
            CancelCommand = new RelayCommand<object>(Cancel);

            _ = LoadLookupData();
        }

        // Methods
        private async Task LoadLookupData()
        {
            // Item Lookup
            var items = await _itemService.GetLookupAsync();
            ItemsLookup = new ObservableCollection<Item>(items);

            // Warehouse Lookup
            var warehouses = await _warehouseService.GetLookupAsync();
            WarehousesLookup = new ObservableCollection<Warehouse>(warehouses);
        }

        private async Task Save()
        {
            if (ItemId == 0)
            {
                MessageBox.Show("품목을 선택하세요.");
                return;
            }

            if (WarehouseId == 0)
            {
                MessageBox.Show("창고를 선택하세요.");
                return;
            }

            if (String.IsNullOrWhiteSpace(TranType))
            {
                MessageBox.Show("거래유형을 선택하세요.");
                return;
            }

            if (Qty <= 0)
            {
                MessageBox.Show("수량은 0보다 커야 합니다.");
                return;
            }

            var inventory = new Inventory
            {
                ItemId = ItemId,
                WarehouseId = WarehouseId,
                TranType = TranType,
                RefType = RefType,
                RefNo = RefNo,
                Qty = Qty,
                UnitCost = UnitCost,
                TranRemark = TranRemark,
                CreatedBy = 1    // 로그인 사용자 ID
            };

            var result = await _inventoryService.InsertAsync(inventory);

            if (result.Code == "MSG0001")
            {
                MessageBox.Show("재고가 등록되었습니다.");
                _navigationService.Navigate(NaviType.InventoryView);
            }
            else
            {
                MessageBox.Show(result.Message);
            }
        }

        private void Cancel(object _)
        {
            _navigationService.Navigate(NaviType.InventoryView);
        }
    }
}