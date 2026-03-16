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
    public class InventoryService : IInventoryService
    {
        private readonly DbExecutor _dbExecutor;

        public InventoryService(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<(List<InventoryDto> Inventories, int TotalCount)> SearchAsync(int? itemId, int? warehouseId, string tranType, string refType, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
        {
            int totalCount = 0;

            var inventories = await _dbExecutor.ExecuteReaderAsync("Inventory_Q",
                cmd =>
                {
                    // Input Parameters
                    cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q";
                    cmd.Parameters.Add("@p_item_id", SqlDbType.Int).Value = itemId.HasValue ? itemId.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_warehouse_id", SqlDbType.Int).Value = warehouseId.HasValue ? warehouseId.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_tran_type", SqlDbType.NVarChar, 8).Value = string.IsNullOrWhiteSpace(tranType) ? DBNull.Value : tranType;
                    cmd.Parameters.Add("@p_ref_type", SqlDbType.NVarChar, 2).Value = string.IsNullOrWhiteSpace(refType) ? DBNull.Value : refType;
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

                    return new InventoryDto
                    {
                        InventoryId = Convert.ToInt32(reader["InventoryId"]),
                        ItemId = Convert.ToInt32(reader["ItemId"]),
                        ItemCode = reader["ItemCode"]?.ToString() ?? string.Empty,
                        ItemName = reader["ItemName"]?.ToString() ?? string.Empty,
                        WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                        WarehouseName = reader["WarehouseName"]?.ToString() ?? string.Empty,
                        TranType = reader["TranType"]?.ToString() ?? string.Empty,
                        RefType = reader["RefType"]?.ToString(),
                        RefNo = reader["RefNo"]?.ToString() ?? string.Empty,
                        Qty = Convert.ToDecimal(reader["Qty"]),
                        UnitCost = reader["UnitCost"] != DBNull.Value ? Convert.ToDecimal(reader["UnitCost"]) : (decimal?)null,
                        TranDate = Convert.ToDateTime(reader["TranDate"]),
                        CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                        UserName = reader["UserName"]?.ToString() ?? string.Empty,
                        TranRemark = reader["TranRemark"]?.ToString() ?? string.Empty,
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    };
                });

            return (inventories, totalCount);
        }

        public async Task<(string Code, string Message)> InsertAsync(Inventory inventory)
        {
            var inventories = await _dbExecutor.ExecuteProcedureAsync("Inventory_S",
                cmd =>
                {
                    // Input Parameters
                    cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "N";
                    cmd.Parameters.Add("@p_item_id", SqlDbType.Int).Value = inventory.ItemId;
                    cmd.Parameters.Add("@p_warehouse_id", SqlDbType.Int).Value = inventory.WarehouseId;
                    cmd.Parameters.Add("@p_tran_type", SqlDbType.NVarChar, 8).Value = inventory.TranType;
                    cmd.Parameters.Add("@p_ref_type", SqlDbType.NVarChar, 2).Value = string.IsNullOrEmpty(inventory.RefType) ? DBNull.Value : inventory.RefType;
                    cmd.Parameters.Add("@p_ref_no", SqlDbType.NVarChar, 14).Value = string.IsNullOrWhiteSpace(inventory.RefNo) ? "" : inventory.RefNo;

                    var qtyParam = cmd.Parameters.Add("@p_qty", SqlDbType.Decimal);
                    qtyParam.Precision = 18;
                    qtyParam.Scale = 4;
                    qtyParam.Value = inventory.Qty;

                    var costParam = cmd.Parameters.Add("@p_unit_cost", SqlDbType.Decimal);
                    costParam.Precision = 18;
                    costParam.Scale = 2;
                    costParam.Value = inventory.UnitCost.HasValue ? inventory.UnitCost.Value : DBNull.Value;

                    var tranDate = inventory.TranDate;
                    if (tranDate < (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue)
                        tranDate = DateTime.Today;
                    cmd.Parameters.Add("@p_tran_date", SqlDbType.DateTime).Value = tranDate;

                    cmd.Parameters.Add("@p_created_by", SqlDbType.Int).Value = inventory.CreatedBy;
                    cmd.Parameters.Add("@p_tran_remark", SqlDbType.NVarChar, 200).Value = string.IsNullOrEmpty(inventory.TranRemark) ? DBNull.Value : inventory.TranRemark;

                    // Output Parameters
                    cmd.Parameters.Add("@p_inventory_id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@p_error_code", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@p_error_str", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@ErrorState", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@ErrorProcedure", SqlDbType.NVarChar, 200).Direction = ParameterDirection.Output;
                });

            string code = inventories["@p_error_code"]?.ToString() ?? "";
            string message = inventories["@p_error_str"]?.ToString() ?? "";

            return (code, message);
        }
    }
}