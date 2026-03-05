using ERP_Project.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Stores
{
    public class UserSessionStore
    {
		public AppUser CurrentUser { get; private set; }

		private bool _isLoggedIn;

		public bool IsLoggedIn
		{
			get { return _isLoggedIn; }
			set {
				if (_isLoggedIn == value)
					return;
                
                _isLoggedIn = value; 
				LoginStateChanged?.Invoke();
			}
		}

		public event Action? LoginStateChanged;

		public void Login()
		{
            IsLoggedIn = true;
        }

		public void Logout()
		{
            IsLoggedIn = false;
		}

		public void SetUser(AppUser user)
		{
			CurrentUser = user;
        }
    }
}