using ERP_Project.Models.Inventory;
using ERP_Project.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Inventorys
{
    public interface IInventoryService
    {
        Task<(List<InventoryDto> Inventories, int TotalCount)> SearchAsync(int? itemId, int? warehouseId, string tranType, string refType, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
    }
}