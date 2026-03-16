using ERP_Project.Models.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Inventorys
{
    public interface ICurrentStockService
    {
        Task<(List<CurrentStockDto> CurrentStocks, int TotalCount)> SearchAsync(int? itemId, int? warehouseId, int pageNumber, int pageSize);
    }
}