using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Models.Master
{
    public class BOM
    {
        public int BOMId { get; set; }
        public int ParentItemId { get; set; }
        public int ChildItemId { get; set; }
        public decimal Qty { get; set; }
        public decimal LossRate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}