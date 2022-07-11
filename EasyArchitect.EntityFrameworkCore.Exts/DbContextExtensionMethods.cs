using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace EasyArchitect.EntityFrameworkCore.Exts
{
    /// <summary>
    /// 擴充方法：取代 .NET 6 不支援的 Database.ExecuteSqlCommand() 方法
    /// </summary>
    public static class DbContextExtensionMethods
    {
        /// <summary>
        /// 從 Entity Framework Core 取得單一值
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<object> SqlQuery(this DbContext context, string sqlStatement)
        {
            DbConnection connection = context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            var command = connection.CreateCommand();
            command.CommandText = sqlStatement;
            return await command.ExecuteScalarAsync();
        }
        /// <summary>
        /// 執行 Sql Statement 方法：取代 .NET 6 不支援的 Database.ExecuteSqlCommand() 方法
        /// </summary>
        /// <param name="context">DbContext</param>
        /// <param name="sqlStatement">要執行的非查詢 Sql 語句，如：INSERT or DELETE or UPDATE</param>
        /// <param name="sqlParameters">DbParames 傳遞 DbParameters</param>
        /// <returns></returns>
        public static async Task ExecuteSqlCommand(this DbContext context, string sqlStatement, DbParames[] sqlParameters = null)
        {
            DbConnection connection = context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            var command = connection.CreateCommand();
            command.CommandText = sqlStatement;
            if (sqlParameters != null && sqlParameters.Length > 0)
            {
                foreach (var p in sqlParameters)
                {
                    DbParameter param = GetParameter(command, p.DbParameName, p.DbParameValue, p.DbParameType);
                    command.Parameters.Add(param);
                }
            }

            await command.ExecuteNonQueryAsync();
        }
        /// <summary>
        /// 取得 DbParameter 執行個體
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbParameter GetParameter(this DbCommand command, string name, object value, DbType type)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

    }
}