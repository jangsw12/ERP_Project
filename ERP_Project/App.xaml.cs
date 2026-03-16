using ERP_Project.Infrastructure.Db;
using ERP_Project.Services.BOM;
using ERP_Project.Services.Customers;
using ERP_Project.Services.Inventorys;
using ERP_Project.Services.Items;
using ERP_Project.Services.Navigation;
using ERP_Project.Services.ProductionOrders;
using ERP_Project.Services.PurchaseOrders;
using ERP_Project.Services.SalesOrders;
using ERP_Project.Services.Users;
using ERP_Project.Services.Warehouses;
using ERP_Project.Stores;
using ERP_Project.ViewModels;
using ERP_Project.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Windows;

namespace ERP_Project
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainView = Services.GetRequiredService<MainView>();
            mainView.Show();

            var navigationService = Services.GetRequiredService<INavigationService>();
            navigationService.Navigate(NaviType.LoginView);
        }

        public new static App Current => (App)Application.Current;

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // ConnectionString
            string connectionString = ERP_Project.Properties.Settings.Default.ConnectionString;

            // Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton(s => new DbExecutor(ERP_Project.Properties.Settings.Default.ConnectionString));
            services.AddSingleton<IItemService, ItemService>();
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<IInventoryService, InventoryService>();
            services.AddSingleton<IProductionOrderService, ProductionOrderService>();
            services.AddSingleton<IPurchaseOrderService, PurchaseOrderService>();
            services.AddSingleton<ISalesOrderService, SalesOrderService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IWarehouseService, WarehouseService>();
            services.AddSingleton<IBOMService, BOMService>();
            services.AddSingleton<ICurrentStockService, CurrentStockService>();

            // Stores
            services.AddSingleton<MainNavigationStore>();
            services.AddSingleton<UserSessionStore>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<ItemViewModel>();
            services.AddTransient<CustomerViewModel>();
            services.AddTransient<WarehouseViewModel>();
            services.AddTransient<BOMViewModel>();
            services.AddTransient<ProductionOrderViewModel>();
            services.AddTransient<PurchaseOrderViewModel>();
            services.AddTransient<SalesOrderViewModel>();
            services.AddTransient<InventoryViewModel>();
            services.AddTransient<ChangePasswordViewModel>();
            services.AddTransient<CurrentStockViewModel>();
            services.AddTransient<InventoryAddViewModel>();
            services.AddTransient<PurchaseOrderAddViewModel>();
            services.AddTransient<PurchaseOrderDetailViewModel>();

            // Views
            services.AddSingleton(s => new MainView()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            return services.BuildServiceProvider();
        }
    }
}