using ERP_Project.Infrastructure.Db;
using ERP_Project.Models.Master;
using ERP_Project.Models.Purchasing;
using ERP_Project.Stores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ERP_Project.Services.PurchaseOrders
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly DbExecutor _dbExecutor;
        private readonly UserSessionStore _userSessionStore;

        public PurchaseOrderService(DbExecutor dbExecutor, UserSessionStore userSessionStore)
        {
            _dbExecutor = dbExecutor;
            _userSessionStore = userSessionStore;
        }

        #region SELECT
        public async Task<(List<PurchaseOrderDto> PurchaseOrders, int TotalCount)> SearchAsync(int? itemId, int? supplierId, int? warehouseId, string? purchaseNumber, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize)
        {
            int totalCount = 0;

            var purchaseOrders = await _dbExecutor.ExecuteReaderAsync("PurchaseOrder_Q",
                cmd =>
                {
                    // Input Parameters
                    cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q";
                    cmd.Parameters.Add("@p_item_id", SqlDbType.Int).Value = itemId.HasValue ? itemId.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_supplier_id", SqlDbType.Int).Value = supplierId.HasValue ? supplierId.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_warehouse_id", SqlDbType.Int).Value = warehouseId.HasValue ? warehouseId.Value : DBNull.Value;
                    cmd.Parameters.Add("@p_purchase_number", SqlDbType.NVarChar, 12).Value = string.IsNullOrWhiteSpace(purchaseNumber) ? DBNull.Value : purchaseNumber;
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

                    return new PurchaseOrderDto
                    {
                        PurchaseOrderDId = Convert.ToInt32(reader["PurchaseOrderDId"]),
                        PurchaseOrderMId = Convert.ToInt32(reader["PurchaseOrderMId"]),
                        PurchaseNumber = reader["PurchaseNumber"]?.ToString() ?? string.Empty,
                        SupplierName = reader["SupplierName"]?.ToString() ?? string.Empty,
                        SupplierId = Convert.ToInt32(reader["SupplierId"]),
                        WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                        WarehouseName = reader["WarehouseName"]?.ToString() ?? string.Empty,
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

            return (purchaseOrders, totalCount);
        }

        public async Task<PurchaseOrderM> GetMasterAsync(int purchaseOrderMId)
        {
            var list = await _dbExecutor.ExecuteReaderAsync("PurchaseOrder_Q",
                 cmd =>
                 {
                     // Input Parameters
                     cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q1";
                     cmd.Parameters.Add("@p_purchase_order_m_id", SqlDbType.Int).Value = purchaseOrderMId;

                     // Output Parameters
                     cmd.Parameters.Add("@p_error_code", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                     cmd.Parameters.Add("@p_row_count", SqlDbType.Int).Direction = ParameterDirection.Output;
                     cmd.Parameters.Add("@p_error_str", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
                     cmd.Parameters.Add("@ErrorState", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                     cmd.Parameters.Add("@ErrorProcedure", SqlDbType.NVarChar, 200).Direction = ParameterDirection.Output;
                 },
                reader => new PurchaseOrderM
                {
                    PurchaseOrderMId = Convert.ToInt32(reader["PurchaseOrderMId"]),
                    PurchaseNumber = reader["PurchaseNumber"]?.ToString(),
                    SupplierId = Convert.ToInt32(reader["SupplierId"]),
                    CustomerName = reader["CustomerName"]?.ToString(),
                    WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                    WarehouseName = reader["WarehouseName"]?.ToString(),
                    OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                    Status = Convert.ToByte(reader["Status"]),
                    Remark = reader["Remark"]?.ToString(),
                });

            return list.FirstOrDefault();
        }

        public async Task<List<PurchaseOrderD>> GetDetailsAsync(int purchaseOrderMId)
        {
            return await _dbExecutor.ExecuteReaderAsync("PurchaseOrder_Q",
                 cmd =>
                 {
                     // Input Parameters
                     cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q2";
                     cmd.Parameters.Add("@p_purchase_order_m_id", SqlDbType.Int).Value = purchaseOrderMId;

                     // Output Parameters
                     cmd.Parameters.Add("@p_error_code", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                     cmd.Parameters.Add("@p_row_count", SqlDbType.Int).Direction = ParameterDirection.Output;
                     cmd.Parameters.Add("@p_error_str", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
                     cmd.Parameters.Add("@ErrorState", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                     cmd.Parameters.Add("@ErrorProcedure", SqlDbType.NVarChar, 200).Direction = ParameterDirection.Output;
                 },
                reader => new PurchaseOrderD
                {
                    PurchaseOrderDId = Convert.ToInt32(reader["PurchaseOrderDId"]),
                    PurchaseOrderMId = Convert.ToInt32(reader["PurchaseOrderMId"]),
                    ItemId = Convert.ToInt32(reader["ItemId"]),
                    ItemCode = reader["ItemCode"]?.ToString(),
                    ItemName = reader["ItemName"]?.ToString(),
                    Qty = Convert.ToDecimal(reader["Qty"]),
                    UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                    Remark = reader["Remark"]?.ToString(),
                });
        }

        public async Task<(string Code, string Message)> ConfirmPurchaseOrderAsync(int purchaseOrderMId)
        {
            var result = await _dbExecutor.ExecuteProcedureAsync("PurchaseOrder_Q",
                cmd =>
                {
                    // Input Parameters
                    cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q3";
                    cmd.Parameters.Add("@p_purchase_order_m_id", SqlDbType.Int).Value = purchaseOrderMId;
                    cmd.Parameters.Add("@p_user_id", SqlDbType.Int).Value = _userSessionStore.CurrentUser.UserId;

                    // Output Parameters
                    cmd.Parameters.Add("@p_error_code", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@p_row_count", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@p_error_str", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@ErrorState", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@ErrorProcedure", SqlDbType.NVarChar, 200).Direction = ParameterDirection.Output;
                });

            string code = result["@p_error_code"]?.ToString() ?? "";
            string message = result["@p_error_str"]?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(message))
                message = code;

            return (code, message);
        }
        #endregion SELECT

        #region INSERT
        public async Task<(string Code, string Message, int PurchaseOrderMId, string PurchaseNumber)> CreateMasterAsync(PurchaseOrderM purchaseOrderM)
        {
            var result = await _dbExecutor.ExecuteProcedureAsync("PurchaseOrder_S", cmd =>
            {
                cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "NM";
                cmd.Parameters.Add("@p_supplier_id", SqlDbType.Int).Value = purchaseOrderM.SupplierId;
                cmd.Parameters.Add("@p_warehouse_id", SqlDbType.Int).Value = purchaseOrderM.WarehouseId;
                cmd.Parameters.Add("@p_order_date", SqlDbType.DateTime).Value = purchaseOrderM.OrderDate == default ? DBNull.Value : purchaseOrderM.OrderDate;
                cmd.Parameters.Add("@p_status", SqlDbType.TinyInt).Value = purchaseOrderM.Status;
                cmd.Parameters.Add("@p_remark", SqlDbType.NVarChar, 100).Value = string.IsNullOrWhiteSpace(purchaseOrderM.Remark) ? DBNull.Value : purchaseOrderM.Remark;
                cmd.Parameters.Add("@p_user_id", SqlDbType.Int).Value = purchaseOrderM.CreatedBy;

                // Output Parameters
                cmd.Parameters.Add("@p_purchase_number", SqlDbType.NVarChar, 12).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_purchase_order_m_id", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_purchase_order_d_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@p_error_code", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_error_str", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorState", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorProcedure", SqlDbType.NVarChar, 200).Direction = ParameterDirection.Output;
            });

            string code = result["@p_error_code"]?.ToString() ?? "";
            string message = result["@p_error_str"]?.ToString() ?? "";
            int purchaseOrderMId = result["@p_purchase_order_m_id"] != DBNull.Value ? Convert.ToInt32(result["@p_purchase_order_m_id"]) : 0;
            string purchaseNumber = result["@p_purchase_number"]?.ToString() ?? "";

            return (code, message, purchaseOrderMId, purchaseNumber);
        }

        public async Task<(string Code, string Message, int PurchaseOrderDId)> CreateDetailAsync(PurchaseOrderD purchaseOrderD)
        {
            var result = await _dbExecutor.ExecuteProcedureAsync("PurchaseOrder_S", cmd =>
            {
                cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "ND";
                cmd.Parameters.Add("@p_purchase_order_m_id", SqlDbType.Int).Value = purchaseOrderD.PurchaseOrderMId;
                cmd.Parameters.Add("@p_item_id", SqlDbType.Int).Value = purchaseOrderD.ItemId;

                var qtyParam = cmd.Parameters.Add("@p_qty", SqlDbType.Decimal);
                qtyParam.Precision = 18;
                qtyParam.Scale = 4;
                qtyParam.Value = purchaseOrderD.Qty;

                var priceParam = cmd.Parameters.Add("@p_unit_price", SqlDbType.Decimal);
                priceParam.Precision = 18;
                priceParam.Scale = 2;
                priceParam.Value = purchaseOrderD.UnitPrice;

                cmd.Parameters.Add("@p_d_remark", SqlDbType.NVarChar, 100).Value = string.IsNullOrWhiteSpace(purchaseOrderD.Remark) ? DBNull.Value : purchaseOrderD.Remark;
                cmd.Parameters.Add("@p_user_id", SqlDbType.Int).Value = purchaseOrderD.CreatedBy;

                // OUTPUT
                cmd.Parameters.Add("@p_purchase_number", SqlDbType.NVarChar, 12).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_purchase_order_d_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@p_error_code", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_error_str", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorState", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorProcedure", SqlDbType.NVarChar, 200).Direction = ParameterDirection.Output;
            });

            string code = result["@p_error_code"]?.ToString() ?? "";
            string message = result["@p_error_str"]?.ToString() ?? "";
            int purchaseOrderDId = result["@p_purchase_order_d_id"] != DBNull.Value ? Convert.ToInt32(result["@p_purchase_order_d_id"]) : 0;

            return (code, message, purchaseOrderDId);
        }

        public async Task<(string Code, string Message, string PurchaseNumber)> SaveAsync(PurchaseOrderM order, List<PurchaseOrderD> details)
        {
            var (code, message, masterId, purchaseNumber) = await CreateMasterAsync(order);
            if (code != "MSG0001")
                return (code, message, string.Empty);

            foreach (var detail in details)
            {
                detail.PurchaseOrderMId = masterId;
                var (dCode, dMessage, _) = await CreateDetailAsync(detail);
                if (dCode != "MSG0001")
                    return (dCode, dMessage, string.Empty);
            }

            return ("MSG0001", "정상 등록 완료", purchaseNumber);
        }
        #endregion INSERT

        #region UPDATE
        public async Task<(string Code, string Message)> UpdateMasterAsync(PurchaseOrderM order)
        {
            var result = await _dbExecutor.ExecuteProcedureAsync("PurchaseOrder_S", cmd =>
            {
                // Input Parameters
                cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "UM";
                cmd.Parameters.Add("@p_purchase_order_m_id", SqlDbType.Int).Value = order.PurchaseOrderMId;
                cmd.Parameters.Add("@p_supplier_id", SqlDbType.Int).Value = order.SupplierId;
                cmd.Parameters.Add("@p_warehouse_id", SqlDbType.Int).Value = order.WarehouseId;
                cmd.Parameters.Add("@p_remark", SqlDbType.NVarChar, 100).Value = string.IsNullOrWhiteSpace(order.Remark) ? DBNull.Value : order.Remark;
                cmd.Parameters.Add("@p_user_id", SqlDbType.Int).Value = _userSessionStore.CurrentUser.UserId;

                // Output Parameters
                cmd.Parameters.Add("@p_purchase_number", SqlDbType.NVarChar, 12).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_purchase_order_d_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@p_error_code", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_error_str", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorState", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorProcedure", SqlDbType.NVarChar, 200).Direction = ParameterDirection.Output;
            });

            string code = result["@p_error_code"]?.ToString() ?? "";
            string message = result["@p_error_str"]?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(message))
                message = code;
            
            return (code, message);
        }

        public async Task<(string Code, string Message)> UpdateDetailAsync(PurchaseOrderD detail)
        {
            var result = await _dbExecutor.ExecuteProcedureAsync("PurchaseOrder_S", cmd =>
            {
                // Input Parameters
                cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "UD";
                cmd.Parameters.Add("@p_purchase_order_d_id", SqlDbType.Int).Value = detail.PurchaseOrderDId;

                var qtyParam = cmd.Parameters.Add("@p_qty", SqlDbType.Decimal);
                qtyParam.Precision = 18;
                qtyParam.Scale = 4;
                qtyParam.Value = detail.Qty;

                cmd.Parameters.Add("@p_d_remark", SqlDbType.NVarChar, 100).Value = string.IsNullOrWhiteSpace(detail.Remark) ? DBNull.Value : detail.Remark;
                cmd.Parameters.Add("@p_user_id", SqlDbType.Int).Value = _userSessionStore.CurrentUser.UserId;

                // Output Parameters
                cmd.Parameters.Add("@p_purchase_number", SqlDbType.NVarChar, 12).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_purchase_order_m_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@p_error_code", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_error_str", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorState", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorProcedure", SqlDbType.NVarChar, 200).Direction = ParameterDirection.Output;
            });

            string code = result["@p_error_code"]?.ToString() ?? "";
            string message = result["@p_error_str"]?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(message))
                message = code;

            return (code, message);
        }
        #endregion
    }
}