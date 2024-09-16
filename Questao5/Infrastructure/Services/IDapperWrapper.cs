using System.Data;

namespace Questao5.Infrastructure.Services
{
	public interface IDapperWrapper
	{
		Task<T> QuerySingleOrDefaultAsync<T>(IDbConnection connection, string sql, object param = null);
		Task<int> ExecuteAsync(IDbConnection connection, string sql, object param = null);
	}
}
