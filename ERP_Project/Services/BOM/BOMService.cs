using ERP_Project.Infrastructure.Db;
using ERP_Project.Models.Master;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.BOM
{
    public class BOMService : IBOMService
    {
        private readonly DbExecutor _dbExecutor;

        public BOMService(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<(List<BOMDto> BOMs, int TotalCount)> SearchAsync(int? parentItemId, int? childItemId, bool? isActive, int pageNumber, int pageSize)
        {
            int totalCount = 0;

            var boms = await _dbExecutor.ExecuteReaderAsync("BOM_Q",
                cmd =>
                {
                    // Input Parameters
                    cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q";
                    cmd.Parameters.Add("@p_parent_item_id", SqlDbType.Int).Value = parentItemId;
                    cmd.Parameters.Add("@p_child_item_id", SqlDbType.Int).Value = childItemId;
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

                    return new BOMDto
                    {
                        BOMId = Convert.ToInt32(reader["BOMId"]),
                        ParentItemId = Convert.ToInt32(reader["ParentItemId"]),
                        ParentItemCode = reader["ParentItemCode"]?.ToString(),
                        ParentItemName = reader["ParentItemName"]?.ToString(),
                        ChildItemId = Convert.ToInt32(reader["ChildItemId"]),
                        ChildItemCode = reader["ChildItemCode"]?.ToString(),
                        ChildItemName = reader["ChildItemName"]?.ToString(),
                        Qty = Convert.ToDecimal(reader["Qty"]),
                        LossRate = Convert.ToDecimal(reader["LossRate"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    };
                });

            return (boms, totalCount);
        }
    }
}