using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ERP_Project.Infrastructure.Db
{
    public class DbExecutor
    {
        private readonly string _connectionString;

        public DbExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<T>> ExecuteReaderAsync<T>(string spName, Action<SqlCommand> parameterSetup, Func<SqlDataReader, T> map)
        {
            var list = new List<T>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            parameterSetup?.Invoke(cmd);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(map(reader));
            }

            return list;
        }

        public async Task<Dictionary<string, object>> ExecuteProcedureAsync(string spName, Action<SqlCommand> parameterSetup)
        {
            var outputs = new Dictionary<string, object>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            parameterSetup?.Invoke(cmd);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            foreach (SqlParameter param in cmd.Parameters)
            {
                if (param.Direction == ParameterDirection.Output || param.Direction == ParameterDirection.InputOutput)
                    outputs[param.ParameterName] = param.Value;
            }

            return outputs;
        }
    }
}