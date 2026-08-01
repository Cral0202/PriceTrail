using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace PriceTrail;

sealed class Program
{
    private const string PipeName = "PriceTrail-SingleInstance-Pipe";
    private const string ShowCommand = "show";

    private static FileStream? _lockFile;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (!TryAcquireInstanceLock())
        {
            // An app instance is already running
            NotifyExistingInstance();
            return;
        }

        StartPipeServer();

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
        }
        finally
        {
            _lockFile?.Dispose();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

    private static bool TryAcquireInstanceLock()
    {
        Directory.CreateDirectory(AppPaths.Runtime);

        try
        {
            // Any other process trying to open this the same way will fail immediately
            _lockFile = new FileStream(AppPaths.InstanceLock, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static void NotifyExistingInstance()
    {
        try
        {
            using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
            client.Connect(500);
            using var writer = new StreamWriter(client) { AutoFlush = true };
            writer.Write(ShowCommand);
        }
        catch
        {
        }
    }

    private static void StartPipeServer()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    using var server = new NamedPipeServerStream(PipeName, PipeDirection.In);
                    await server.WaitForConnectionAsync();

                    using var reader = new StreamReader(server);
                    var message = await reader.ReadToEndAsync();

                    if (message == ShowCommand)
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            if (Application.Current is App app)
                                app.ShowMainWindow();
                        });
                    }
                }
                catch
                {
                    await Task.Delay(500);
                }
            }
        });
    }
}
