using Serilog;

namespace SMARTscan_DataProcessor.Data
{
    public static class AppLogger
    {
        public static void ConfigApp()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("SmartscanLog.log")
                .CreateLogger();
        }

        public static void LogInformation(string logmessage)
        {
            Log.Information(logmessage);
        }

        public static void LogWarning(string logmessage)
        {
            Log.Warning(logmessage);
        }

    }
}
