using System;
using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using BackEndAutomation.Utilities;
using Reqnroll;
using RestSharp;

namespace BackEndAutomation.Tests.BBDTests
{
    [Binding]
    public class StockStepDefinitions
    {
        private RestCalls restCalls = new RestCalls();
        private ResponseDataExtractors extractResponseData = new ResponseDataExtractors();

        private readonly ScenarioContext _scenarioContext;
        private ExtentTest _test;
        private List<double> numbers = new List<double>();
        private string calculateTimeMessageResponse;

        public StockStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [Given("get stock price and last divident price for nVidia")]
        public void GivenGetStockPriceAndLastDividentPriceForNVidia()
        {
            // stock price
            numbers.Add(15.23);
            // divident per share
            numbers.Add(5.18);
            // number of times per year given divident
            numbers.Add(4);

            UtilitiesMethods.LogMessage(
                message: "Data for nVidia stocks is get",
                scenarioContext: _scenarioContext,
                LogStatuses.Info);
        }
        //baseURL
        // https://testapis-usjm.onrender.com/
        //endpoint
        // calculate
        //params
        // ?targetMonthlyIncome=1000
        // &dividendFrequency=4
        // &stockPrice=15.23
        // &dividendPerShare=5.18
        // &monthlyInvestment=100
        [When("calculate the time for return investment of {int} for {int} per month")]
        public void WhenCalculateTheTimeForReturnInvestmentOfForPerMonth(int targetIncome, int investPerMonth)
        {
            double dividendFrequency = numbers[2];
            double stockPrice = numbers[0];
            double dividendPerShare = numbers[1];

            string baseUrl = "https://testapis-usjm.onrender.com";
            string endpoint = $"/calculate?targetMonthlyIncome={targetIncome}" +
                $"&dividendFrequency={dividendFrequency}&stockPrice={stockPrice}" +
                $"&dividendPerShare={dividendPerShare}&monthlyInvestment={investPerMonth}";

            RestResponse response = restCalls.generalRestCall(baseUrl, endpoint, Method.Get);
            calculateTimeMessageResponse = extractResponseData.ExtractStockMessage(response.Content);

            UtilitiesMethods.LogMessage(
                message: "The calculating the time is done",
                scenarioContext: _scenarioContext,
                LogStatuses.Info);
        }

        [Then("the needed time is {int} years and {int} months")]
        public void ThenTheNeededTimeIsYearsAndMonths(int years, int months)
        {
            string expectedMessage = $"{years} years and {months} months";

            UtilitiesMethods.AssertEqual(
                actual: calculateTimeMessageResponse,
                expected: expectedMessage,
                message: "The calculate time is different than the expected",
                scenarioContext: _scenarioContext);

            UtilitiesMethods.LogMessage(
                message: "The calculate time is same as the expected",
                scenarioContext: _scenarioContext);
        }
    }
}
