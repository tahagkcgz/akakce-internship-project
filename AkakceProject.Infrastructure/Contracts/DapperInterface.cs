using System.Data;

namespace AkakceProject.Infrastructure.Contracts
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);
        Task<T> QuerySingleAsync<T>(string sql, object param = null);
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null);
        Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, TResult>(string sql, Func<T1, T2, T3, TResult> map, object param = null, string splitOn = null);
        Task ExecuteAsync(string sql, object param = null);
        Task ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null);
    }
}
