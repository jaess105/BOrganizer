using System.Reflection;

namespace DB.Core;

public interface IDbScriptsProvider
{
    IEnumerable<Assembly> GetAssemblyWithScripts();
}