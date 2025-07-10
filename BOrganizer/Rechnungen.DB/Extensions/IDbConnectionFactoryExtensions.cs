using System.Data;
using DB.Core;

namespace Rechnungen.DB.Extensions;

// ReSharper disable once InconsistentNaming
public static class IDbConnectionFactoryExtensions
{
    public static IDbConnection CreateOpenConnection(this IDbConnectionFactory self)
    {
        IDbConnection conn = self.CreateConnection();
        conn.Open();
        return conn;
    }
}