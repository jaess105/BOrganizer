using System.Data;
using DB.Core;
using Npgsql;
using RepoDb;

namespace Rechnungen.DB;

public class DbConnectionFactory : IDbConnectionFactory
{
    private static DbConnectionFactory? _instance;
    private readonly string _connectionString;

    private DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public static DbConnectionFactory Init(string connectionString)
    {
        if (_instance is not null) { return _instance; }

        GlobalConfiguration.Setup().UsePostgreSql();
        _instance = new DbConnectionFactory(connectionString);
        return _instance;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

    public Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
        => Task.FromResult(CreateConnection());
}