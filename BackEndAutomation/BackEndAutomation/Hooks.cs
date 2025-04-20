using BackEndAutomation.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;
using System.Collections;

namespace BackEndAutomation
{
    [Binding]
    public sealed class Hooks
    {

        private readonly ScenarioContext _scenarioContext;
        private IWebDriver _driver;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            ExtentManager.InitReport();
        }

        [BeforeScenario("UI")]
        public void BeforeUIScenario()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized"); // Отваряне на цял екран
            options.AddArgument("--disable-notifications"); // Деактивиране на изскачащи нотификации

            _driver = new ChromeDriver(options);

            // Запазване на WebDriver в контекста на сценариите
            _scenarioContext["WebDriver"] = _driver;
        }

        [AfterScenario("UI")]
        public void AfterUIScenario()
        {
            if (_scenarioContext.ContainsKey("WebDriver"))
            {
                _driver = (IWebDriver)_scenarioContext["WebDriver"];

                if (_scenarioContext.TestError != null) // Ако има грешка в теста
                {
                    TakeScreenshot(_scenarioContext.ScenarioInfo.Title);
                }

                _driver.Quit();
            }
        }

        //[BeforeScenario("@tag1", Order =-9)]
        [BeforeScenario("@tag1")]
        public void BeforeScenarioWithTag()
        {
            Console.WriteLine("Do something only when scenario is tagged with '@tag1'");
        }

        [BeforeScenario(Order = 1)]
        public void FirstBeforeScenario()
        {
            Console.WriteLine("Do something before every scenario, the lowest order is executed first");
        }

        [BeforeScenario(Order = -99)]
        public void NegativeBeforeScenario()
        {
            Console.WriteLine("FIRST FIRST BEFORE");
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            var еxtentTest = ExtentManager.CreateTest("Scenario: " + GetScenarioName(_scenarioContext));
            _scenarioContext["ExtentTest"] = еxtentTest;
            UtilitiesMethods.LogMessage("Starting scenario", _scenarioContext);
            Logger.Log.Info("Starting scenario...");
        }

        [BeforeStep]
        public void BeforeStep()
        {
            UtilitiesMethods.LogMessage("Start executing Step: " + _scenarioContext.StepContext.StepInfo.Text, _scenarioContext, LogStatuses.Debug);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            Logger.Log.Info("Scenario finished.");
        }

        [AfterTestRun]
        public static void FlushReport()
        {
            ExtentManager.FlushReport();
        }

        private static string GetScenarioName(ScenarioContext context)
        {
            string scenarioParameters = string.Empty;

            foreach (DictionaryEntry entry in context.ScenarioInfo.Arguments)
            {
                scenarioParameters += @"""" + entry.Value.ToString() + @"""" + ", ";
            }

            if (!string.IsNullOrEmpty(scenarioParameters))
            {
                scenarioParameters = "(" + scenarioParameters + "null)";
            }

            string scenarioName = context.ScenarioInfo.Title + scenarioParameters;

            return scenarioName;
        }

        private void TakeScreenshot(string scenarioName)
        {
            try
            {
                ITakesScreenshot screenshotDriver = _driver as ITakesScreenshot;
                Screenshot screenshot = screenshotDriver.GetScreenshot();

                string screenshotsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
                if (!Directory.Exists(screenshotsDirectory))
                {
                    Directory.CreateDirectory(screenshotsDirectory);
                }

                string filePath = Path.Combine(screenshotsDirectory, $"{scenarioName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                screenshot.SaveAsFile(filePath);

                Console.WriteLine($"Screenshot saved: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to take screenshot: {ex.Message}");
            }
        }
    }
}