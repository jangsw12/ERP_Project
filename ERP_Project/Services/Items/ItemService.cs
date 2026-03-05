using ERP_Project.Infrastructure.Db;
using ERP_Project.Models.Master;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Items
{
    public class ItemService : IItemService
    {
        private readonly DbExecutor _dbExecutor;

        public ItemService(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<(List<Item> Items, int TotalCount)> SearchAsync(string itemCode, string itemName, string itemType, bool? isActive, int pageNumber, int pageSize)
        {
            int totalCount = 0;

            var items = await _dbExecutor.ExecuteReaderAsync("Item_Q",
                cmd =>
                {
                    // Input Parameters
                    cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q";
                    cmd.Parameters.Add("@p_item_code", SqlDbType.NVarChar, 20).Value = string.IsNullOrWhiteSpace(itemCode) ? DBNull.Value : itemCode;
                    cmd.Parameters.Add("@p_item_name", SqlDbType.NVarChar, 50).Value = string.IsNullOrWhiteSpace(itemName) ? DBNull.Value : itemName;
                    cmd.Parameters.Add("@p_item_type", SqlDbType.NVarChar, 20).Value = string.IsNullOrWhiteSpace(itemType) ? DBNull.Value : itemType;
                    cmd.Parameters.Add("@p_is_active", SqlDbType.Bit).Value = isActive.HasValue ? isActive.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_page_number", SqlDbType.Int).Value = pageNumber;
                    cmd.Parameters.Add("@p_page_size", SqlDbType.Int).Value = pageSize;

                    // Output Parameters
                    cmd.Parameters.Add("@p_error_code", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@p_row_count", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@p_error_str", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@ErrorState", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@ErrorProcedure", SqlDbType.NVarChar, 200).Direction = ParameterDirection.Output;
                },
                reader =>
                {
                    if (totalCount == 0 && reader["TotalCount"] != DBNull.Value)
                        totalCount = Convert.ToInt32(reader["TotalCount"]);

                    return new Item
                    {
                        ItemId = Convert.ToInt32(reader["ItemId"]),
                        ItemCode = reader["ItemCode"]?.ToString(),
                        ItemName = reader["ItemName"]?.ToString(),
                        ItemType = reader["ItemType"]?.ToString(),
                        Unit = reader["Unit"]?.ToString(),
                        StandardCost = Convert.ToDecimal(reader["StandardCost"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    };
                });

            return (items, totalCount);
        }

        public async Task<List<Item>> GetLookupAsync()
        {
            var result = await SearchAsync(null, null, null, null, 1, 1000);
            return result.Items;
        }
    }
}