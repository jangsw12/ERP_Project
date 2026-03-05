using ERP_Project.Commands;
using ERP_Project.Models.Master;
using ERP_Project.Services.Customers;
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
    public class CustomerViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;

        public ObservableCollection<Customer> Customers { get; set; }
        private Customer _selectedCustomer;

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> CustomerTypes { get; }

        private string _searchBusinessNumber;

        public string SearchBusinessNumber
        {
            get { return _searchBusinessNumber; }
            set
            {
                _searchBusinessNumber = value;
                OnPropertyChanged();
            }
        }

        private string _searchCustomerName;

        public string SearchCustomerName
        {
            get { return _searchCustomerName; }
            set
            {
                _searchCustomerName = value;
                OnPropertyChanged();
            }
        }

        private string _searchCustomerType;

        public string SearchCustomerType
        {
            get { return _searchCustomerType; }
            set
            {
                _searchCustomerType = value;
                OnPropertyChanged();
            }
        }

        private int _totalCount;

        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value;
                OnPropertyChanged();
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
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }

        public CustomerViewModel(ICustomerService customerService)
        {
            _customerService = customerService;

            Customers = new ObservableCollection<Customer>();
            CustomerTypes = new ObservableCollection<string> { "", "Supplier", "Customer", "Both" };

            SearchCommand = new RelayCommand<object>(async _ => await Search());
            AddCommand = new RelayCommand<object>(_ => Add());
            SaveCommand = new RelayCommand<object>(async _ => await Save());
            DeleteCommand = new RelayCommand<object>(async _ => await Delete());
            NextPageCommand = new RelayCommand<object>(async _ => await NextPage(), _ => PageNumber < TotalPages);
            PrevPageCommand = new RelayCommand<object>(async _ => await PrevPage(), _ => PageNumber > 1);

            _ = Search();
        }

        // Methods
        private async Task Search()
        {
            if (PageNumber < 1)
                PageNumber = 1;

            var result = await _customerService.SearchAsync(SearchBusinessNumber, SearchCustomerName, SearchCustomerType, true, PageNumber, PageSize);

            Customers.Clear();

            foreach (var customer in result.Customers)
                Customers.Add(customer);

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

        private void Add()
        {

        }

        private async Task Save()
        {

        }

        private async Task Delete()
        {

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

        private void Validate(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.BusinessNumber))
                throw new Exception("거래처코드는 필수입니다.");

            if (string.IsNullOrWhiteSpace(customer.CustomerName))
                throw new Exception("거래처명은 필수입니다.");

            if (string.IsNullOrWhiteSpace(customer.CustomerType))
                throw new Exception("유형은 필수입니다.");
        }
    }
}