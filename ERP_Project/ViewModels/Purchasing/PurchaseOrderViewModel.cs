using ERP_Project.Commands;
using ERP_Project.Models.Master;
using ERP_Project.Models.Purchasing;
using ERP_Project.Services.Customers;
using ERP_Project.Services.Items;
using ERP_Project.Services.Navigation;
using ERP_Project.Services.PurchaseOrders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERP_Project.ViewModels
{
    public class PurchaseOrderViewModel : ViewModelBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IItemService _itemService;
        private readonly ICustomerService _customerService;
        private readonly INavigationService _navigationService;

        public ObservableCollection<PurchaseOrderDto> PurchaseOrders { get; set; }
        private PurchaseOrderDto _selectedPurchaseOrder;

        public PurchaseOrderDto SelectedPurchaseOrder
        {
            get { return _selectedPurchaseOrder; }
            set
            {
                _selectedPurchaseOrder = value;
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

        private ObservableCollection<Customer> _suppliersLookup;

        public ObservableCollection<Customer> SuppliersLookup
        {
            get { return _suppliersLookup; }
            set
            {
                _suppliersLookup = value;
                OnPropertyChanged();
            }
        }

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

        private int? _searchSupplierId;

        public int? SearchSupplierId
        {
            get { return _searchSupplierId; }
            set
            {
                _searchSupplierId = value;
                OnPropertyChanged();
            }
        }

        private string _searchPurchaseNumber;

        public string SearchPurchaseNumber
        {
            get { return _searchPurchaseNumber; }
            set { 
                _searchPurchaseNumber = value;
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
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand GoCustomerCommand { get; }

        public PurchaseOrderViewModel(IPurchaseOrderService purchaseOrderService, IItemService itemService, ICustomerService customerService, INavigationService navigationService)
        {
            _purchaseOrderService = purchaseOrderService;
            _itemService = itemService;
            _customerService = customerService;
            _navigationService = navigationService;

            PurchaseOrders = new ObservableCollection<PurchaseOrderDto>();
            ItemsLookup = new ObservableCollection<Item>();
            SuppliersLookup = new ObservableCollection<Customer>();
           
            // 날짜 기본값 설정
            var today = DateTime.Today;
            SearchDateFrom = new DateTime(today.Year, today.Month, 1);
            SearchDateTo = SearchDateFrom.Value.AddMonths(1).AddDays(-1);

            SearchCommand = new RelayCommand<object>(async _ => await Search());
            NextPageCommand = new RelayCommand<object>(async _ => await NextPage(), _ => PageNumber < TotalPages);
            PrevPageCommand = new RelayCommand<object>(async _ => await PrevPage(), _ => PageNumber > 1);
            GoCustomerCommand = new RelayCommand<object>(GoCustomer);

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

            // Customer Lookup
            var customers = await _customerService.GetLookupAsync();

            SuppliersLookup.Clear();
            SuppliersLookup.Add(new Customer
            {
                CustomerId = 0,
                CustomerName = null
            });

            foreach (var customer in customers)
                SuppliersLookup.Add(customer);
        }

        private async Task Search()
        {
            var result = await _purchaseOrderService.SearchAsync(SearchItemId == 0 ? null : SearchItemId, SearchSupplierId == 0 ? null : SearchSupplierId, SearchPurchaseNumber, SearchDateFrom, SearchDateTo, PageNumber, PageSize);

            PurchaseOrders.Clear();

            foreach (var pur in result.PurchaseOrders)
                PurchaseOrders.Add(pur);

            TotalCount = result.TotalCount;

            if (PageNumber > TotalPages && TotalPages > 0)
            {
                PageNumber = 1;
                await Search();
                return;
            }
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

        private void GoCustomer(object _)
        {
            _navigationService.Navigate(NaviType.CustomerView);
        }
    }
}