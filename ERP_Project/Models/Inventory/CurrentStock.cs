using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Models.Inventory
{
    public class CurrentStock
    {
        public int CurrentStockId { get; set; }
        public int ItemId { get; set; }
        public int WarehouseId { get; set; }
        public decimal Qty { get; set; }
        public DateTime? LastTranDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public byte[]? rowversion { get; set; }              // RowVersion
    }
}