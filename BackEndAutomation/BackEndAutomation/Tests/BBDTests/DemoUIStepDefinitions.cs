using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using Reqnroll;
using BackEndAutomation.Utilities;
using SeleniumExtras.WaitHelpers;

namespace BackEndAutomation.Tests.BBDTests
{
    [Binding]
    public class DemoUIStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IWebDriver _driver;

        public DemoUIStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _driver = (IWebDriver)_scenarioContext["WebDriver"];
        }

        [Given(@"I open the Google homepage")]
        public void GivenIOpenTheGoogleHomepage()
        {
            _driver.Navigate().GoToUrl("https://www.google.com");
        }

        [When(@"I search for ""(.*)""")]
        public void WhenISearchFor(string searchText)
        {
            IWebElement searchBox = _driver.FindElement(By.Name("q"));
            searchBox.SendKeys(searchText);
            searchBox.SendKeys(Keys.Enter);
        }

        [Then(@"the first result should contain ""(.*)""")]
        public void ThenTheFirstResultShouldContain(string expectedText)
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            IWebElement firstResult = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("h3")));

            UtilitiesMethods.AssertEqual(
                actual: firstResult.Text.Contains(expectedText),
                expected: true,
                message: "First result does not contain expected text.",
                scenarioContext: _scenarioContext);
        }
    }
}
