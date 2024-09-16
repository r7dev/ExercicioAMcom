using Dapper;
using System.Data;

namespace Questao5.Infrastructure.Services
{
	public class DapperWrapper : IDapperWrapper
	{
		public async Task<T> QuerySingleOrDefaultAsync<T>(IDbConnection connection, string sql, object param = null)
		{
			return await connection.QuerySingleOrDefaultAsync<T>(sql, param);
		}
		public async Task<int> ExecuteAsync(IDbConnection connection, string sql, object param = null)
		{
			return await connection.ExecuteAsync(sql, param);
		}
	}
}
