using System.Reflection;
using DB.Core;

namespace Rechnungen.DB;

public class DbScriptsProvider : IDbScriptsProvider
{
    public IEnumerable<Assembly> GetAssemblyWithScripts()
    {
        return [typeof(DbScriptsProvider).Assembly];
    }
}