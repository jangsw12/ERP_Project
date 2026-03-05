using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Models.Sales
{
    public class SalesOrderDto
    {
        public int SalesOrderDId { get; set; }
        public int SalesOrderMId { get; set; }
        public string SalesNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public Decimal Qty { get; set; }
        public Decimal UnitPrice { get; set; }
        public Decimal Amount { get; set; }
        public string? Remark { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}