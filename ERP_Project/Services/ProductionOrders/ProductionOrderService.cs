using ERP_Project.Infrastructure.Db;
using ERP_Project.Models.Production;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.ProductionOrders
{
    public class ProductionOrderService : IProductionOrderService
    {
        private readonly DbExecutor _dbExecutor;

        public ProductionOrderService(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<(List<ProductionOrderDto> ProductionOrders, int TotalCount)> SearchAsync(int? finishedItemId, int? componentItemId, string? productionNumber, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
        {
            int totalCount = 0;

            var productionOrders = await _dbExecutor.ExecuteReaderAsync("ProductionOrder_Q",
                cmd =>
                {
                    // Input Parameters
                    cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q";
                    cmd.Parameters.Add("@p_finished_item_id", SqlDbType.Int).Value = finishedItemId.HasValue ? finishedItemId.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_component_item_id", SqlDbType.Int).Value = componentItemId.HasValue ? componentItemId.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_production_number", SqlDbType.NVarChar, 12).Value = string.IsNullOrWhiteSpace(productionNumber) ? DBNull.Value : productionNumber;
                    cmd.Parameters.Add("@p_date_from", SqlDbType.DateTime).Value = dateFrom.HasValue ? dateFrom.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_date_to", SqlDbType.DateTime).Value = dateTo.HasValue ? dateTo.Value : DBNull.Value;
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

                    return new ProductionOrderDto
                    {
                        ProductionOrderDId = Convert.ToInt32(reader["ProductionOrderDId"]),
                        ProductionOrderMId = Convert.ToInt32(reader["ProductionOrderMId"]),
                        ProductionNumber = reader["ProductionNumber"]?.ToString() ?? string.Empty,
                        FinishedItemId = Convert.ToInt32(reader["FinishedItemId"]),
                        ItemCode = reader["ItemCode"]?.ToString() ?? string.Empty,
                        ItemName = reader["ItemName"]?.ToString() ?? string.Empty,
                        ComponentItemId = Convert.ToInt32(reader["ComponentItemId"]),
                        ComponentItemCode = reader["ComponentItemCode"]?.ToString() ?? string.Empty,
                        ComponentItemName = reader["ComponentItemName"]?.ToString() ?? string.Empty,
                        RequiredQty = Convert.ToDecimal(reader["RequiredQty"]),
                        UsedQty = Convert.ToDecimal(reader["UsedQty"]),
                        PlanQty = Convert.ToDecimal(reader["PlanQty"]),
                        ProducedQty = Convert.ToDecimal(reader["ProducedQty"]),
                        Status = Convert.ToByte(reader["Status"]),
                        StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : (DateTime?)null,
                        EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : (DateTime?)null,
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    };
                });

            return (productionOrders, totalCount);
        }
    }
}