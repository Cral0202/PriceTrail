using System;
using System.IO;

namespace PriceTrail;

public static class AppPaths
{
    public static string Root => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PriceTrail");

    public static string Data => Path.Combine(Root, "Data");

    public static string Database => Path.Combine(Data, "pricetrail.db");

    public static string Playwright => Path.Combine(Root, "Playwright");

    public static string Runtime => Path.Combine(Root, "Runtime");

    public static string InstanceLock => Path.Combine(Runtime, "instance.lock");
}
