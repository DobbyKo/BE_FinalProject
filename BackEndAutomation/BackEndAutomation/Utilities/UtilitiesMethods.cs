using AventStack.ExtentReports;
using NUnit.Framework;
using Reqnroll;

namespace BackEndAutomation.Utilities
{
    public class UtilitiesMethods
    {
        public static void AssertEqual<T>(T expected, T actual, string message, ScenarioContext scenarioContext)
        {
            ExtentTest _test = scenarioContext.Get<ExtentTest>("ExtentTest");

            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                Logger.Log.Error(message + Environment.NewLine + $"Expected: {expected}, but was: {actual}");
                _test.Log(Status.Fail, message + Environment.NewLine + $"Expected: {expected}, but was: {actual}");
                Assert.Fail(message + Environment.NewLine + $"Expected: {expected}, but was: {actual}");
            }
        }

        public static void LogMessage(
            string message,
            ScenarioContext scenarioContext,
            LogStatuses status = LogStatuses.Info)
        {
            ExtentTest _test = scenarioContext.Get<ExtentTest>("ExtentTest");
            if (status == LogStatuses.Info)
            {
                _test.Log(Status.Info, message);
                Logger.Log.Info(message);
            }
            else if (status == LogStatuses.Warning)
            {
                _test.Log(Status.Warning, message);
                Logger.Log.Warn(message);
            }
            else if (status == LogStatuses.Debug)
            {
                _test.Log(Status.Info, message);
                Logger.Log.Debug(message);
            }
        }
    }

    public enum LogStatuses
    {
        Info,
        Warning,
        Debug
    }
}
