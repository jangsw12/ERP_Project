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
        Task<(List<PurchaseOrderDto> PurchaseOrders, int TotalCount)> SearchAsync(int? itemId, int? supplierId, int? warehouseId, string? purchaseNumber, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
        Task<(string Code, string Message, int PurchaseOrderMId, string PurchaseNumber)> CreateMasterAsync(PurchaseOrderM purchaseOrderM);
        Task<(string Code, string Message, int PurchaseOrderDId)> CreateDetailAsync(PurchaseOrderD purchaseOrderD);
        Task<(string Code, string Message, string PurchaseNumber)> SaveAsync(PurchaseOrderM order, List<PurchaseOrderD> details);
        Task<PurchaseOrderM> GetMasterAsync(int purchaseOrderMId);
        Task<List<PurchaseOrderD>> GetDetailsAsync(int purchaseOrderMId);
        Task<(string Code, string Message)> ConfirmPurchaseOrderAsync(int purchaseOrderMId);
        Task<(string Code, string Message)> UpdateMasterAsync(PurchaseOrderM order);
        Task<(string Code, string Message)> UpdateDetailAsync(PurchaseOrderD detail);
    }
}