using System.Reflection;

namespace PriceTrail;

public static class Constants
{
    public const string LatestReleaseApi = "https://api.github.com/repos/Cral0202/PriceTrail/releases/latest";

    public static string AppVersion =>
        Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion
            .Split('+')[0]
        ?? "Unknown";
}
