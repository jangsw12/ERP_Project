using ERP_Project.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.SalesOrders
{
    public interface ISalesOrderService
    {
        Task<(List<SalesOrderDto> SalesOrders, int TotalCount)> SearchAsync(int? itemId, int? customerId, string? salesNumber, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
    }
}