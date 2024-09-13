using Dapper;
using System.Data;

namespace Questao5Test.Mock
{
	public class DbExecutor
	{
		private readonly IDbConnection _dbConnection;

		public DbExecutor(IDbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public Task<int> ExecuteAsync(string sql, object param)
		{
			return _dbConnection.ExecuteAsync(sql, param);
		}
	}
}
