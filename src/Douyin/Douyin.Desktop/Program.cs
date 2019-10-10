using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.Platform;
using Avalonia.ReactiveUI;

namespace Douyin.Desktop
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // TODO: Make this work with GTK/Skia/Cairo depending on command-line args
            // again.
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        }

        /// <summary>
        /// This method is needed for IDE previewer infrastructure
        /// </summary>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .LogToDebug()
                .UsePlatformDetect()
                .UseReactiveUI();

        private static void ConfigureAssetAssembly(AppBuilder builder)
        {
            AvaloniaLocator.CurrentMutable
                .GetService<IAssetLoader>()
                .SetDefaultAssembly(typeof(App).Assembly);
        }
    }
}
