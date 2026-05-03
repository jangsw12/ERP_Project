using ERP_Project.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Users
{
    public interface IUserService
    {
        Task<AppUser?> LoginAsync(string loginId, string password);
        Task<string> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}