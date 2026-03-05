using ERP_Project.Commands;
using ERP_Project.Models.Master;
using ERP_Project.Models.Production;
using ERP_Project.Models.Purchasing;
using ERP_Project.Services.Customers;
using ERP_Project.Services.Items;
using ERP_Project.Services.Navigation;
using ERP_Project.Services.ProductionOrders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERP_Project.ViewModels
{
    public class ProductionOrderViewModel : ViewModelBase
    {
        private readonly IProductionOrderService _productionOrderService;
        private readonly IItemService _itemService;
        private readonly INavigationService _navigationService;

        public ObservableCollection<ProductionOrderDto> ProductionOrders { get; set; }
        private ProductionOrderDto _selectedProductionOrder;

        public ProductionOrderDto SelectedProductionOrder
        {
            get { return _selectedProductionOrder; }
            set
            {
                _selectedProductionOrder = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Item> _finishedItemsLookup;

        public ObservableCollection<Item> FinishedItemsLookup
        {
            get { return _finishedItemsLookup; }
            set
            {
                _finishedItemsLookup = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Item> _componentItemsLookup;

        public ObservableCollection<Item> ComponentItemsLookup
        {
            get { return _componentItemsLookup; }
            set
            {
                _componentItemsLookup = value;
                OnPropertyChanged();
            }
        }

        // 검색 조건
        private int? _searchFinishedItemId;

        public int? SearchFinishedItemId
        {
            get { return _searchFinishedItemId; }
            set
            {
                _searchFinishedItemId = value;
                OnPropertyChanged();
            }
        }

        private int? _searchComponentItemId;

        public int? SearchComponentItemId
        {
            get { return _searchComponentItemId; }
            set
            {
                _searchComponentItemId = value;
                OnPropertyChanged();
            }
        }

        private string _searchProductionNumber;

        public string SearchProductionNumber
        {
            get { return _searchProductionNumber; }
            set
            {
                _searchProductionNumber = value;
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
        public ICommand GoBOMCommand { get; }

        public ProductionOrderViewModel(IProductionOrderService productionOrderService, IItemService itemService, ICustomerService customerService, INavigationService navigationService)
        {
            _productionOrderService = productionOrderService;
            _itemService = itemService;
            _navigationService = navigationService;

            ProductionOrders = new ObservableCollection<ProductionOrderDto>();
            FinishedItemsLookup = new ObservableCollection<Item>();
            ComponentItemsLookup = new ObservableCollection<Item>();

            // 날짜 기본값 설정
            var today = DateTime.Today;
            SearchDateFrom = new DateTime(today.Year, today.Month, 1);
            SearchDateTo = SearchDateFrom.Value.AddMonths(1).AddDays(-1);

            SearchCommand = new RelayCommand<object>(async _ => await Search());
            NextPageCommand = new RelayCommand<object>(async _ => await NextPage(), _ => PageNumber < TotalPages);
            PrevPageCommand = new RelayCommand<object>(async _ => await PrevPage(), _ => PageNumber > 1);
            GoBOMCommand = new RelayCommand<object>(GoBOM);

            _ = LoadLookupData();
            _ = Search();
        }

        // Methods
        private async Task LoadLookupData()
        {
            // Finished Item Lookup
            var finishedItems = await _itemService.GetLookupAsync();

            FinishedItemsLookup.Clear();
            FinishedItemsLookup.Add(new Item
            {
                ItemId = 0,
                ItemName = ""
            });

            foreach (var finishedItem in finishedItems)
                FinishedItemsLookup.Add(finishedItem);

            // Component Item Lookup
            var componentItems = await _itemService.GetLookupAsync();

            ComponentItemsLookup.Clear();
            ComponentItemsLookup.Add(new Item
            {
                ItemId = 0,
                ItemName = ""
            });

            foreach (var componentItem in componentItems)
                ComponentItemsLookup.Add(componentItem);
        }

        private async Task Search()
        {
            var result = await _productionOrderService.SearchAsync(SearchFinishedItemId == 0 ? null : SearchFinishedItemId, SearchComponentItemId == 0 ? null : SearchComponentItemId, SearchProductionNumber, SearchDateFrom, SearchDateTo, PageNumber, PageSize);

            ProductionOrders.Clear();

            foreach (var pro in result.ProductionOrders)
                ProductionOrders.Add(pro);

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

        private void GoBOM(object _)
        {
            _navigationService.Navigate(NaviType.BOMView);
        }
    }
}