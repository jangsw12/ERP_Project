using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Models.Production
{
    public class ProductionOrderD
    {
        public int ProductionOrderDId { get; set; }
        public int ProductionOrderMId { get; set; }
        public int ItemId { get; set; }
        public decimal RequiredQty { get; set; }
        public decimal UsedQty { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}