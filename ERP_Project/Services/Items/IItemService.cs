using ERP_Project.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Items
{
    public interface IItemService
    {
        Task<(List<Item> Items, int TotalCount)> SearchAsync(string itemCode, string itemName, string itemType, bool? isActive, int pageNumber, int pageSize);
        Task<List<Item>> GetLookupAsync();
    }
}