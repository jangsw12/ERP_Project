using ERP_Project.Infrastructure.Db;
using ERP_Project.Models.Master;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly DbExecutor _dbExecutor;

        public CustomerService(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }
        public async Task<(List<Customer> Customers, int TotalCount)> SearchAsync(string busniessNumber, string customerName, string customerType, bool? isActive, int pageNumber, int pageSize)
        {
            int totalCount = 0;

            var customers = await _dbExecutor.ExecuteReaderAsync("Customer_Q",
                cmd =>
                {
                    // Input Parameters
                    cmd.Parameters.Add("@p_work_type", SqlDbType.VarChar, 10).Value = "Q";
                    cmd.Parameters.Add("@p_business_number", SqlDbType.NVarChar, 15).Value = string.IsNullOrWhiteSpace(busniessNumber) ? DBNull.Value : busniessNumber;
                    cmd.Parameters.Add("@p_customer_name", SqlDbType.NVarChar, 40).Value = string.IsNullOrWhiteSpace(customerName) ? DBNull.Value : customerName;
                    cmd.Parameters.Add("@p_customer_type", SqlDbType.NVarChar, 20).Value = string.IsNullOrWhiteSpace(customerType) ? DBNull.Value : customerType;
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

                    return new Customer
                    {
                        CustomerId = Convert.ToInt32(reader["CustomerId"]),
                        CustomerName = reader["CustomerName"]?.ToString(),
                        CustomerType = reader["CustomerType"]?.ToString(),
                        BusinessNumber = reader["BusinessNumber"]?.ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    };
                });

            return (customers, totalCount);
        }

        public async Task<List<Customer>> GetLookupAsync()
        {
            var result = await SearchAsync(null, null, null, true, 1, 1000);
            return result.Customers;
        }
    }
}