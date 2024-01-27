using AkakceProject.Infrastructure.Contracts;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

public class DapperContext : IDapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("AkakceAppCon");
    }

    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
    {
        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<T>(sql, param);
        }
    }

    public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null)
    {
        using (var connection = CreateConnection())
        {
            return await connection.QuerySingleOrDefaultAsync<T>(sql, param);
        }
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object param = null)
    {
        using (var connection = CreateConnection())
        {
            return await connection.QuerySingleAsync<T>(sql, param);
        }
    }

    public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, TResult>(string sql, Func<T1, T2, T3, TResult> map, object param = null, string splitOn = null)
    {
        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync(sql, map, param, splitOn: splitOn);
        }
    }

    public async Task ExecuteAsync(string sql, object param = null)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(sql, param);
        }
    }

    public async Task ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            await connection.ExecuteAsync(sql, param, transaction);
        }
    }
}