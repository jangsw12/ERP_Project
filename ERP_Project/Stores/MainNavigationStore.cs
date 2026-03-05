using ERP_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Stores
{
    public class MainNavigationStore
    {
		private ViewModelBase? _currentViewModel;

		public ViewModelBase? CurrentViewModel
        {
			get { return _currentViewModel; }
			set {
                if (_currentViewModel == value)
                    return;

				_currentViewModel = value;
                CurrentViewModelChanged?.Invoke();
			}
		}

        public event Action? CurrentViewModelChanged;
    }
}