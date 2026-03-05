using ERP_Project.Models.Purchasing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.PurchaseOrders
{
    public interface IPurchaseOrderService
    {
        Task<(List<PurchaseOrderDto> PurchaseOrders, int TotalCount)> SearchAsync(int? itemId, int? supplierId, string? purchaseNumber, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
    }
}