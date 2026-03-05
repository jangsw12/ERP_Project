using ERP_Project.Commands;
using ERP_Project.Models.Master;
using ERP_Project.Services.BOM;
using ERP_Project.Services.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERP_Project.ViewModels
{
    public class BOMViewModel : ViewModelBase
    {
        private readonly IBOMService _bomService;
        private readonly IItemService _itemService;

        public ObservableCollection<BOMDto> BOMs { get; set; }
        private BOMDto _selectedBOM;

        public BOMDto SelectedBOM
        {
            get { return _selectedBOM; }
            set
            {
                _selectedBOM = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Item> _parentItemsLookup;

        public ObservableCollection<Item> ParentItemsLookup
        {
            get { return _parentItemsLookup; }
            set
            {
                _parentItemsLookup = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Item> _childItemsLookup;

        public ObservableCollection<Item> ChildItemsLookup
        {
            get { return _childItemsLookup; }
            set
            {
                _childItemsLookup = value;
                OnPropertyChanged();
            }
        }

        // 검색 조건
        private int? _searchParentItemId;

        public int? SearchParentItemId
        {
            get { return _searchParentItemId; }
            set
            {
                _searchParentItemId = value;
                OnPropertyChanged();
            }
        }

        private int? _searchChildItemId;

        public int? SearchChildItemId
        {
            get { return _searchChildItemId; }
            set
            {
                _searchChildItemId = value;
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

        public BOMViewModel(IBOMService bomService, IItemService itemService)
        {
            _bomService = bomService;
            _itemService = itemService;

            BOMs = new ObservableCollection<BOMDto>();
            ParentItemsLookup = new ObservableCollection<Item>();
            ChildItemsLookup = new ObservableCollection<Item>();

            SearchCommand = new RelayCommand<object>(async _ => await Search());
            NextPageCommand = new RelayCommand<object>(async _ => await NextPage(), _ => PageNumber < TotalPages);
            PrevPageCommand = new RelayCommand<object>(async _ => await PrevPage(), _ => PageNumber > 1);

            _ = LoadLookupData();
            _ = Search();
        }

        // Methods
        private async Task LoadLookupData()
        {
            // Parent Item Lookup
            var parentItems = await _itemService.GetLookupAsync();

            ParentItemsLookup.Clear();
            ParentItemsLookup.Add(new Item
            {
                ItemId = 0,
                ItemName = ""
            });

            foreach (var parentItem in parentItems)
                ParentItemsLookup.Add(parentItem);

            // Child Item Lookup
            var childItems = await _itemService.GetLookupAsync();

            ChildItemsLookup.Clear();
            ChildItemsLookup.Add(new Item
            {
                ItemId = 0,
                ItemName = ""
            });

            foreach (var childItem in childItems)
                ChildItemsLookup.Add(childItem);
        }

        private async Task Search()
        {
            if (PageNumber < 1)
                PageNumber = 1;

            var result = await _bomService.SearchAsync(SearchParentItemId == 0 ? null : SearchParentItemId, SearchChildItemId == 0 ? null : SearchChildItemId, true, PageNumber, PageSize);

            BOMs.Clear();

            foreach (var bom in result.BOMs)
                BOMs.Add(bom);

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