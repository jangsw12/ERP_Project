using ERP_Project.Infrastructure.Db;
using ERP_Project.Models.Sales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.SalesOrders
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly DbExecutor _dbExecutor;

        public SalesOrderService(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<(List<SalesOrderDto> SalesOrders, int TotalCount)> SearchAsync(int? itemId, int? customerId, string? salesNumber, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
        {
            int totalCount = 0;

            var salesOrders = await _dbExecutor.ExecuteReaderAsync("SalesOrder_Q",
                cmd =>
                {
                    // Input Parameters
                    cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q";
                    cmd.Parameters.Add("@p_item_id", SqlDbType.Int).Value = itemId.HasValue ? itemId.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_customer_id", SqlDbType.Int).Value = customerId.HasValue ? customerId.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_sales_number", SqlDbType.NVarChar, 12).Value = string.IsNullOrWhiteSpace(salesNumber) ? DBNull.Value : salesNumber;
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

                    return new SalesOrderDto
                    {
                        SalesOrderDId = Convert.ToInt32(reader["SalesOrderDId"]),
                        SalesOrderMId = Convert.ToInt32(reader["SalesOrderMId"]),
                        SalesNumber = reader["SalesNumber"]?.ToString() ?? string.Empty,
                        CustomerName = reader["CustomerName"]?.ToString() ?? string.Empty,
                        CustomerId = Convert.ToInt32(reader["CustomerId"]),
                        ItemId = Convert.ToInt32(reader["ItemId"]),
                        ItemCode = reader["ItemCode"]?.ToString() ?? string.Empty,
                        ItemName = reader["ItemName"]?.ToString() ?? string.Empty,
                        Qty = Convert.ToDecimal(reader["Qty"]),
                        UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                        Amount = Convert.ToDecimal(reader["Amount"]),
                        Remark = reader["Remark"]?.ToString() ?? string.Empty,
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    };
                });

            return (salesOrders, totalCount);
        }
    }
}