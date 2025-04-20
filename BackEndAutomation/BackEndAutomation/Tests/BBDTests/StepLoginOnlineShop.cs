using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using OpenQA.Selenium;
using Reqnroll;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndAutomation.Tests.BBDTests
{
    [Binding]
    public class StepLoginOnlineShop
    {

        private RestCalls restCalls = new RestCalls();
        private ResponseDataExtractors extractResponseData = new ResponseDataExtractors();
        private RestResponse userLoginResponse, userProfileDetailsResponse, userFollowResponse, userUnfollowResponse;
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest _test;

        public StepLoginOnlineShop(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [When(@"online shop login with ""(.*)"" username and ""(.*)"" password")]
        public void OnlineShopLogin(string username, string password)
        {
            RestResponse response = restCalls.LoginOnlineShopCall(username, password);
            string tokenValue = extractResponseData.ExtractLoggedInUserToken(response.Content, "access_token");

            _scenarioContext.Add("UserToken",tokenValue);
        }
        [Then(@"validate user is logged in")]
        public void ValidationLoggedInUser()
        {
            bool isTokenExtracted = string.IsNullOrEmpty(_scenarioContext.Get<string>("UserToken"));
            Utilities.UtilitiesMethods.AssertEqual(
                false,
                isTokenExtracted,
                "Token is not extracted or user is not logged in",
                _scenarioContext);
        }
    }
}
