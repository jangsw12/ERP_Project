using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Models.Sales
{
    public class SalesOrderD
    {
        public int SalesOrderDId { get; set; }
        public int SalesOrderMId { get; set; }
        public int ItemId { get; set; }
        public Decimal Qty { get; set; }
        public Decimal UnitPrice { get; set; }
        public Decimal Amount => Qty * UnitPrice;
        public string? Remark { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}