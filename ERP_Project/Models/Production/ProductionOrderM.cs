using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Models.Production
{
    public class ProductionOrderM
    {
        public int ProductionOrderMId { get; set; }
        public string ProductionNumber { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public decimal PlanQty { get; set; }
        public decimal ProducedQty { get; set; }
        public byte Status { get; set; }                    // tinyint
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public byte[]? rowversion { get; set; }              // RowVersion
    }
}