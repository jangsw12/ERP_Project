using ERP_Project.Models.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.ProductionOrders
{
    public interface IProductionOrderService
    {
        Task<(List<ProductionOrderDto> ProductionOrders, int TotalCount)> SearchAsync(int? finishedItemId, int? componentItemId, string? productionNumber, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
    }
}