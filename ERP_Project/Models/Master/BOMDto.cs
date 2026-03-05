using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Models.Master
{
    public class BOMDto : BOM
    {
        public string ParentItemCode { get; set; } = string.Empty;
        public string ParentItemName { get; set; } = string.Empty;
        public string ChildItemCode { get; set; } = string.Empty;
        public string ChildItemName { get; set; } = string.Empty;
    }
}