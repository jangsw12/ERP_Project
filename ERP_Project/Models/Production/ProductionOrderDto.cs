using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Models.Production
{
    public class ProductionOrderDto
    {
        public int ProductionOrderDId { get; set; }
        public int ProductionOrderMId { get; set; }
        public string ProductionNumber { get; set; } = string.Empty;
        public int FinishedItemId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public int ComponentItemId { get; set; }
        public string ComponentItemCode { get; set; } = string.Empty;
        public string ComponentItemName { get; set; } = string.Empty;
        public Decimal RequiredQty { get; set; }
        public Decimal UsedQty { get; set; }
        public Decimal PlanQty { get; set; }
        public Decimal ProducedQty { get; set; }
        public byte Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}