using ERP_Project.Commands;
using ERP_Project.Models.Master;
using ERP_Project.Services.Items;
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
    public class ItemViewModel : ViewModelBase
    {
        private readonly IItemService _itemService;

        public ObservableCollection<Item> Items { get; set; }
        private Item _selectedItem;

        public Item SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ItemTypes { get; }

        private string _searchItemCode;

        public string SearchItemCode
        {
            get { return _searchItemCode; }
            set
            {
                _searchItemCode = value;
                OnPropertyChanged();
            }
        }

        private string _searchItemName;

        public string SearchItemName
        {
            get { return _searchItemName; }
            set
            {
                _searchItemName = value;
                OnPropertyChanged();
            }
        }

        private string _searchItemType;

        public string SearchItemType
        {
            get { return _searchItemType; }
            set
            {
                _searchItemType = value;
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

        public ItemViewModel(IItemService itemService)
        {
            _itemService = itemService;

            Items = new ObservableCollection<Item>();
            ItemTypes = new ObservableCollection<string> { "", "RawMaterial", "FinishedProduct" };

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

            var result = await _itemService.SearchAsync(SearchItemCode, SearchItemName, SearchItemType, true, PageNumber, PageSize);

            Items.Clear();

            foreach (var item in result.Items)
                Items.Add(item);

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