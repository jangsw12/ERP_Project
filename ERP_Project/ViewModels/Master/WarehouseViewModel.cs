using ERP_Project.Commands;
using ERP_Project.Models.Master;
using ERP_Project.Services.Items;
using ERP_Project.Services.Warehouses;
using ERP_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERP_Project.ViewModels
{
    public class WarehouseViewModel : ViewModelBase
    {
        private readonly IWarehouseService _warehouseService;

        public ObservableCollection<Warehouse> Warehouses { get; set; }
        private Warehouse _selectedWarehouse;

        public Warehouse SelectedWarehouse
        {
            get { return _selectedWarehouse; }
            set
            {
                _selectedWarehouse = value;
                OnPropertyChanged();
            }
        }

        private string _searchWarehouseName;

        public string SearchWarehouseName
        {
            get { return _searchWarehouseName; }
            set
            {
                _searchWarehouseName = value;
                OnPropertyChanged();
            }
        }

        private string _searchLocation;

        public string SearchLocation
        {
            get { return _searchLocation; }
            set
            {
                _searchLocation = value;
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

        public WarehouseViewModel(IWarehouseService itemService)
        {
            _warehouseService = itemService;

            Warehouses = new ObservableCollection<Warehouse>();

            SearchCommand = new RelayCommand<object>(async _ => await Search());
            NextPageCommand = new RelayCommand<object>(async _ => await NextPage(), _ => PageNumber < TotalPages);
            PrevPageCommand = new RelayCommand<object>(async _ => await PrevPage(), _ => PageNumber > 1);

            _ = Search();
        }

        // Methods
        private async Task Search()
        {
            if (PageNumber < 1)
                PageNumber = 1;

            var result = await _warehouseService.SearchAsync(SearchWarehouseName, SearchLocation, true, PageNumber, PageSize);

            Warehouses.Clear();

            foreach (var warehouse in result.Warehouses)
                Warehouses.Add(warehouse);

            TotalCount = result.TotalCount;

            if (PageNumber > TotalPages && TotalPages > 0)
            {
                PageNumber = 1;
                await Search();
                return;
            }

            OnPropertyChanged(nameof(TotalPages));
            CommandManager.InvalidateRequerySuggested();
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
    }
}