using Microsoft.Extensions.Logging;

namespace ChromecastIframe.Helpers;

public static class LoggerHelper
{
    public static ILogger<T> SetupLogger<T>()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        var logger = loggerFactory.CreateLogger<T>();

        return logger;
    }
}
