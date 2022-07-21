using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Reflection;

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
#pragma warning disable CS8603 // 可能有 Null 參考傳回。
            return await command.ExecuteScalarAsync();
#pragma warning restore CS8603 // 可能有 Null 參考傳回。
        }
        /// <summary>
        /// 提供查詢資料庫方法。
        /// 說明：由你的 Entity Framework Provider 決定使用何種資料庫，並傳入適當的 SQL 敘述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">DbContext</param>
        /// <param name="sqlStatement">SQL 敘述</param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> SqlQuery<T>(this DbContext context, string sqlStatement)
            where T : class, new()
        {
            return await SqlQuery<T>(context, sqlStatement, null);
        }
        /// <summary>
        /// 提供查詢資料庫方法。
        /// 說明：由你的 Entity Framework Provider 決定使用何種資料庫，並傳入適當的 SQL 敘述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">DbContext</param>
        /// <param name="sqlStatement">SQL 敘述</param>
        /// <param name="sqlParameters">Sql Parameters (陣列型態)</param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> SqlQuery<T>(this DbContext context, string sqlStatement, DbParames[]? sqlParameters = null)
            where T : class, new()
        {
            DbConnection connection = context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            var command = connection.CreateCommand();
            command.CommandText = sqlStatement;
            BuildSqlParameters(sqlParameters, command);

            DbDataReader reader = await command.ExecuteReaderAsync();

            List<T> result = new List<T>();

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public |
                                BindingFlags.Instance |
                                BindingFlags.Default);

            List<string> columns = new List<string>();
            for(int i=0; i<reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }

            while(await reader.ReadAsync())
            {
                T row = Activator.CreateInstance<T>();

                foreach (PropertyInfo pInfo in properties)
                {
                    var foundColumn = columns
                        .Where(c => c.ToLower() == pInfo.Name.ToLower())
                        .FirstOrDefault();

                    if(foundColumn != null)
                    {
                        if(!reader.IsDBNull(foundColumn))
                        {
                            switch (reader.GetFieldType(foundColumn).FullName)
                            {
                                case "System.Int16":
                                    pInfo.SetValue(row, reader.GetInt16(foundColumn));
                                    break;
                                case "System.Int32":
                                    pInfo.SetValue(row, reader.GetInt32(foundColumn));
                                    break;
                                case "System.Decimal":
                                    pInfo.SetValue(row, reader.GetInt64(foundColumn));
                                    break;
                                case "System.DateTime":
                                    pInfo.SetValue(row, reader.GetDateTime(foundColumn));
                                    break;
                                case "System.Boolean":
                                    pInfo.SetValue(row, reader.GetBoolean(foundColumn));
                                    break;
                                // 補上其他型態：
                                // case "":
                                //  break;
                                case "System.String":
                                default:
                                    pInfo.SetValue(row, reader.GetString(foundColumn));
                                    break;

                            }
                        }
                    }
                }

                result.Add(row);
            }

            return result;
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
            BuildSqlParameters(sqlParameters, command);

            await command.ExecuteNonQueryAsync();
        }
        /// <summary>
        /// 建立 SqlParameters
        /// </summary>
        /// <param name="sqlParameters"></param>
        /// <param name="command"></param>
        private static void BuildSqlParameters(DbParames[]? sqlParameters, DbCommand command)
        {
            if (sqlParameters != null && sqlParameters.Length > 0)
            {
                foreach (var p in sqlParameters)
                {
                    DbParameter param = GetParameter(command, p.DbParameName, p.DbParameValue, p.DbParameType);
                    command.Parameters.Add(param);
                }
            }
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