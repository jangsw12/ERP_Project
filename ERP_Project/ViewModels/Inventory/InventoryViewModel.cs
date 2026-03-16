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
using System.Windows.Input;

namespace ERP_Project.ViewModels
{
    public class InventoryViewModel : ViewModelBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IItemService _itemService;
        private readonly IWarehouseService _warehouseService;
        private readonly INavigationService _navigationService;

        public ObservableCollection<InventoryDto> Inventories { get; set; }
        private InventoryDto _selectedInventory;

        public InventoryDto SelectedInventory
        {
            get { return _selectedInventory; }
            set
            {
                _selectedInventory = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Item> _itemsLookup;

        public ObservableCollection<Item> ItemsLookup
        {
            get { return _itemsLookup; }
            set { 
                _itemsLookup = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Warehouse> _warehousesLookup;

        public ObservableCollection<Warehouse> WarehousesLookup
        {
            get { return _warehousesLookup; }
            set { 
                _warehousesLookup = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> TranTypes { get; set; }
        public ObservableCollection<string> RefTypes { get; set; }

        // 검색 조건
        private int? _searchItemId;

        public int? SearchItemId
        {
            get { return _searchItemId; }
            set
            {
                _searchItemId = value;
                OnPropertyChanged();
            }
        }

        private int? _searchWarehouseId;

        public int? SearchWarehouseId
        {
            get { return _searchWarehouseId; }
            set
            {
                _searchWarehouseId = value == 0 ? null : value;
                OnPropertyChanged();
            }
        }

        private string _searchTranType;

        public string SearchTranType
        {
            get { return _searchTranType; }
            set
            {
                _searchTranType = value;
                OnPropertyChanged();
            }
        }

        private string _searchRefType;

        public string SearchRefType
        {
            get { return _searchRefType; }
            set
            {
                _searchRefType = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _searchDateFrom;

        public DateTime? SearchDateFrom
        {
            get { return _searchDateFrom; }
            set
            {
                _searchDateFrom = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _searchDateTo;

        public DateTime? SearchDateTo
        {
            get { return _searchDateTo; }
            set
            {
                _searchDateTo = value;
                OnPropertyChanged();
            }
        }

        // Paging
        private int _totalCount;

        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        private int _pageNumber = 1;

        public int PageNumber
        {
            get { return _pageNumber; }
            set
            {
                _pageNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        private int _pageSize = 20;

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    OnPropertyChanged();
                    PageNumber = 1;     // 페이지 초기화
                    _ = Search();       // 자동 재조회
                }
            }
        }

        public List<int> PageSizeOptions { get; } = new() { 10, 20, 50, 100 };
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

        // Commands
        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand GoWarehouseCommand { get; }

        public InventoryViewModel(IInventoryService inventoryService, IItemService itemService, IWarehouseService warehouseService, INavigationService navigationService)
        {
            _inventoryService = inventoryService;
            _itemService = itemService;
            _warehouseService = warehouseService;
            _navigationService = navigationService;

            Inventories = new ObservableCollection<InventoryDto>();
            ItemsLookup = new ObservableCollection<Item>();
            WarehousesLookup = new ObservableCollection<Warehouse>();
            TranTypes = new ObservableCollection<string> { "", "IN", "OUT", "PROD_IN", "PROD_OUT" };
            RefTypes = new ObservableCollection<string> { "", "PO", "SO", "PR" };

            // 날짜 기본값 설정
            var today = DateTime.Today;
            SearchDateFrom = new DateTime(today.Year, today.Month, 1);
            SearchDateTo = SearchDateFrom.Value.AddMonths(1).AddDays(-1);

            SearchCommand = new RelayCommand<object>(async _ => await Search());
            AddCommand = new RelayCommand<object>(Add);
            NextPageCommand = new RelayCommand<object>(async _ => await NextPage(), _ => PageNumber < TotalPages);
            PrevPageCommand = new RelayCommand<object>(async _ => await PrevPage(), _ => PageNumber > 1);
            GoWarehouseCommand = new RelayCommand<object>(GoWarehouse);

            _ = LoadLookupData();
            _ = Search();
        }

        // Methods
        private async Task LoadLookupData()
        {
            // Item Lookup
            var items = await _itemService.GetLookupAsync();

            ItemsLookup.Clear();
            ItemsLookup.Add(new Item
            {
                ItemId = 0,
                ItemName = ""
            });

            foreach (var item in items)
                ItemsLookup.Add(item);

            // Warehouse Lookup
            var warehouses = await _warehouseService.GetLookupAsync();

            WarehousesLookup.Clear();
            WarehousesLookup.Add(new Warehouse
            {
                WarehouseId = 0,
                WarehouseName = null
            });

            foreach (var warehouse in warehouses)
                WarehousesLookup.Add(warehouse);
        }

        private async Task Search()
        {
            var result = await _inventoryService.SearchAsync(SearchItemId == 0 ? null : SearchItemId, SearchWarehouseId, SearchTranType, SearchRefType, SearchDateFrom, SearchDateTo, PageNumber, PageSize);

            Inventories.Clear();

            foreach (var inv in result.Inventories)
                Inventories.Add(inv);

            TotalCount = result.TotalCount;

            if (PageNumber > TotalPages && TotalPages > 0)
            {
                PageNumber = 1;
                await Search();
                return;
            }
        }

        private void Add(object _)
        {
            _navigationService.Navigate(NaviType.InventoryAddView);
        }

        private async Task NextPage()
        {
            if (PageNumber >= TotalPages)
                return;

            PageNumber++;
            await Search();
        }

        private async Task PrevPage()
        {
            if (PageNumber <= 1)
                return;

            PageNumber--;
            await Search();
        }

        private void GoWarehouse(object _)
        {
            _navigationService.Navigate(NaviType.WarehouseView);
        }
    }
}