using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Models.Inventory
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public int ItemId { get; set; }
        public int WarehouseId { get; set; }
        public string TranType { get; set; } = string.Empty;
        public string? RefType { get; set; }
        public int? RefId { get; set; }
        public decimal Qty { get; set; }
        public decimal? UnitCost { get; set; }
        public DateTime TranDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}