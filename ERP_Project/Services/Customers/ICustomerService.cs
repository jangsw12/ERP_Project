using ERP_Project.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Customers
{
    public interface ICustomerService
    {
        Task<(List<Customer> Customers, int TotalCount)> SearchAsync(string busniessNumber, string customerName, string customerType, bool? isActive, int pageNumber, int pageSize);
        Task<List<Customer>> GetLookupAsync();
    }
}