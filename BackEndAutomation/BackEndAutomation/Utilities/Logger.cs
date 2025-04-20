using NLog;

namespace BackEndAutomation.Utilities
{
    public static class Logger
    {
        public static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    }
}
