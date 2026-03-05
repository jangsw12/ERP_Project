using ERP_Project.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.BOM
{
    public interface IBOMService
    {
        Task<(List<BOMDto> BOMs, int TotalCount)> SearchAsync(int? parentItemId, int? childItemId, bool? isActive, int pageNumber, int pageSize);
    }
}