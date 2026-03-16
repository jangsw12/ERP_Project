using ERP_Project.Infrastructure.Db;
using ERP_Project.Models.Inventory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Inventorys
{
    public class CurrentStockService : ICurrentStockService
    {
        private readonly DbExecutor _dbExecutor;

        public CurrentStockService(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<(List<CurrentStockDto> CurrentStocks, int TotalCount)> SearchAsync(int? itemId, int? warehouseId, int pageNumber, int pageSize)
        {
            int totalCount = 0;

            var currentStocks = await _dbExecutor.ExecuteReaderAsync("CurrentStock_Q",
                cmd =>
                {
                    // Input Parameters
                    cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q";
                    cmd.Parameters.Add("@p_item_id", SqlDbType.Int).Value = itemId.HasValue ? itemId.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_warehouse_id", SqlDbType.Int).Value = warehouseId.HasValue ? warehouseId.Value : DBNull.Value;
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

                    return new CurrentStockDto
                    {
                        ItemId = Convert.ToInt32(reader["ItemId"]),
                        ItemCode = reader["ItemCode"]?.ToString() ?? string.Empty,
                        ItemName = reader["ItemName"]?.ToString() ?? string.Empty,
                        WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                        WarehouseName = reader["WarehouseName"]?.ToString() ?? string.Empty,
                        Qty = Convert.ToDecimal(reader["Qty"]),
                        LastTranDate = Convert.ToDateTime(reader["LastTranDate"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    };
                });

            return (currentStocks, totalCount);
        }
    }
}