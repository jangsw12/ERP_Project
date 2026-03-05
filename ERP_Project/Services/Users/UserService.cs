using ERP_Project.Infrastructure.Db;
using ERP_Project.Models.System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_Project.Services.Users
{
    public class UserService : IUserService
    {
        private readonly DbExecutor _dbExecutor;

        public UserService(DbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<AppUser?> LoginAsync(string loginId, string password)
        {
            var users = await _dbExecutor.ExecuteReaderAsync("AppUser_Q", cmd =>
            {
                // Input Parameters
                cmd.Parameters.AddWithValue("@p_login_id", loginId);
                cmd.Parameters.AddWithValue("@p_password", password);

                // Output Parameters
                cmd.Parameters.Add("@p_error_code", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_row_count", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_error_str", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorState", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorProcedure", SqlDbType.NVarChar, 200).Direction = ParameterDirection.Output;
            },
                reader => new AppUser
                {
                    UserId = Convert.ToInt32(reader["UserId"]),
                    LoginId = reader["LoginId"].ToString(),
                    UserName = reader["UserName"].ToString(),
                    Role = reader["Role"].ToString()
                }
            );

            return users.FirstOrDefault();
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var result = await _dbExecutor.ExecuteProcedureAsync("AppUser_S", cmd =>
            {
                cmd.Parameters.AddWithValue("@p_user_id", userId);
                cmd.Parameters.AddWithValue("@p_current_password", currentPassword);
                cmd.Parameters.AddWithValue("@p_new_password", newPassword);

                cmd.Parameters.Add("@p_error_code", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_row_count", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_error_str", SqlDbType.NVarChar, 1000).Direction = ParameterDirection.Output;
            });

            return result["@p_error_code"]?.ToString() == "SUCCESS";
        }
    }
}