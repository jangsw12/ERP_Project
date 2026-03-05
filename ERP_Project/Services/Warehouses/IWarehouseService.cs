using ERP_Project.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Warehouses
{
    public interface IWarehouseService
    {
        Task<(List<Warehouse> Warehouses, int TotalCount)> SearchAsync(string warehouseName, string location, bool? isActive, int pageNumber, int pageSize);
        Task<List<Warehouse>> GetLookupAsync();
    }
}