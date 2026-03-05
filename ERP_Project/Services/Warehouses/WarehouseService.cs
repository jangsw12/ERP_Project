using ERP_Project.Infrastructure.Db;
using ERP_Project.Models.Master;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Warehouses
{
    public class WarehouseService : IWarehouseService
    {
        private readonly DbExecutor _dbExecutor;

        public WarehouseService(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<(List<Warehouse> Warehouses, int TotalCount)> SearchAsync(string warehouseName, string location, bool? isActive, int pageNumber, int pageSize)
        {
            int totalCount = 0;

            var warehouses = await _dbExecutor.ExecuteReaderAsync("Warehouse_Q",
                cmd =>
                {
                    // Input Parameters
                    cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q";
                    cmd.Parameters.Add("@p_warehouse_name", SqlDbType.NVarChar, 30).Value = string.IsNullOrWhiteSpace(warehouseName) ? DBNull.Value : warehouseName;
                    cmd.Parameters.Add("@p_location", SqlDbType.NVarChar, 50).Value = string.IsNullOrWhiteSpace(location) ? DBNull.Value : location;
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

                    return new Warehouse
                    {
                        WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                        WarehouseName = reader["WarehouseName"]?.ToString(),
                        Location = reader["Location"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    };
                });

            return (warehouses, totalCount);
        }

        public async Task<List<Warehouse>> GetLookupAsync()
        {
            var result = await SearchAsync(null, null, true, 1, 1000);
            return result.Warehouses;
        }
    }
}