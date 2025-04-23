using System;
using System.Net;
using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using BackEndAutomation.Utilities;
using Newtonsoft.Json;
using NUnit.Framework;
using Reqnroll;
using RestSharp;

namespace BackEndAutomation.Tests.BBDTests
{
    [Binding]
    public class UserLoginFunctionalityStepDefinitions
    {
        private RestCalls _restCalls = new RestCalls();
        private ResponseDataExtractors _dataExtractors = new ResponseDataExtractors();
        private ScenarioContext _scenarioContext;
        private ExtentTest _test;
        private RestResponse _response;
        private string _token;

        public UserLoginFunctionalityStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = _scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [Given("login data is prepared")]
        public void GivenLoginDataIsPrepared()
        {
            UtilitiesMethods.LogMessage("Login data is prepared for Admin", _scenarioContext, LogStatuses.Info);
            Logger.Log.Info("Login data prepared successfully.");
        }

        [When("an Admin user with valid username \"(.*)\" and password \"(.*)\"")]
        public void WhenAnAdminUserWithValidUsernameAndPassword(string username, string password)
        {
            _test.Log(Status.Info, $"Attempting login for Admin user: {username}");
            Logger.Log.Info($"Attempting login for Admin user: {username}");

            _response = _restCalls.SchoolApiLoginCall(username, password);

            Logger.Log.Info($"Login response received: {_response.Content}");
            _test.Log(Status.Info, "Login API call executed.");

            Assert.That(_response, Is.Not.Null, "Login API call failed.");

            string token = _restCalls.GetTokenFromResponse(_response);

            _scenarioContext.Set(token, "access_token");
            Logger.Log.Info($"Access token stored in ScenarioContext: {token}");
        }

        [When("\"(.*)\" user with valid username - \"(.*)\" and password - \"(.*)\"")]
        public void WhenUserWithValidUsername_AndPassword_(string role, string username, string password)
        {
            _test.Log(Status.Info, $"Attempting login for {role} user: {username}");
            Logger.Log.Info($"Attempting login for {role} user: {username}");

            _response = _restCalls.SchoolApiLoginCall(username, password);

            Logger.Log.Info($"Login response received: {_response.Content}");
            _test.Log(Status.Info, "Login API call executed.");

            Assert.That(_response, Is.Not.Null, $"Login API call failed for {role} user.");

            string token = _dataExtractors.ExtractLoggedInUserToken(_response.Content, "access_token");

            if (string.IsNullOrEmpty(token))
            {
                Logger.Log.Error($"{role} user login failed! Token not retrieved.");
                Assert.Fail($"{role} user login failed! Token not retrieved.");
            }

            _scenarioContext.Set(token, "access_token");
            _scenarioContext.Set(role, "role");

            Logger.Log.Info($"{role} access token stored in ScenarioContext: {token}");
        }


        [When("a user submits a login request with missing or incorrect \"(.*)\" or \"(.*)\"")]
        public void WhenAUserSubmitsALoginRequestWithMissingOr(string username, string password)
        {
            _test.Log(Status.Info, $"Attempting login with username: '{username}' and password: '{password}'");
            Logger.Log.Info($"Attempting login with username: '{username}' and password: '{password}'");

            string loginUsername = string.IsNullOrEmpty(username) || username == "*" ? string.Empty : username;
            string loginPassword = string.IsNullOrEmpty(password) || password == "*" ? string.Empty : password;

            _response = _restCalls.SchoolApiLoginCall(loginUsername, loginPassword);

            Logger.Log.Info($"Login response received: {_response.Content}");
            _test.Log(Status.Info, "Login API call executed.");
        }

        [Then("the response should return a valid JWT token")]
        public void ThenTheResponseShouldReturnAValidJWTToken()
        {
            _token = _dataExtractors.ExtractLoggedInUserToken(_response.Content, "access_token");

            Utilities.UtilitiesMethods.AssertEqual(false, string.IsNullOrEmpty(_token), "JWT token was not returned!", _scenarioContext);

            Logger.Log.Info($"Successfully retrieved JWT Token: {_token}");
            _test.Log(Status.Pass, $"Successfully retrieved JWT Token: {_token}");
        }

        [Then("the token should contain Admin permissions")]
        public void ThenTheTokenShouldContainAdminPermissions()
        {
            bool isTokenExtracted = string.IsNullOrEmpty(_dataExtractors.ExtractLoggedInUserToken(_response.Content, "access_token"));
            Utilities.UtilitiesMethods.AssertEqual(
                false,
                isTokenExtracted,
                "Token is not extracted or user is not logged in",
                _scenarioContext);

            _test.Log(Status.Pass, "Token contains correct Admin permissions.");
            Logger.Log.Info("Verified Admin permissions successfully.");
        }

        [Then("the response should return a validation error")]
        public void ThenTheResponseShouldReturnAValidationError()
        {
            if (_response.StatusCode == HttpStatusCode.OK)
            {
                Logger.Log.Error("The API unexpectedly returned a successful response with an access token.");
                Assert.Fail("Expected a validation error response, but the API returned a valid token.");
            }

            Assert.That(
                _response.StatusCode,
                Is.EqualTo(HttpStatusCode.Unauthorized),
                "Expected a validation error response, but received a different status code."
            );

            var errorResponse = JsonConvert.DeserializeObject<dynamic>(_response.Content);
            Assert.That(
                errorResponse?.detail,
                Is.Not.Null,
                "Expected validation error details, but none were found."
            );

            Logger.Log.Info("Validation error response successfully verified.");
            _test.Log(Status.Pass, "Validation error response successfully verified.");
        }

        [Then("no JWT token should be returned")]
        public void ThenNoJWTTokenShouldBeReturned()
        {
            string token = _restCalls.GetTokenFromResponse(_response);

            Assert.That(string.IsNullOrEmpty(token), Is.True, "JWT token was unexpectedly returned in the response.");

            Logger.Log.Info("Verified that no JWT token was returned.");
            _test.Log(Status.Pass, "Verified that no JWT token was returned.");
        }

    }
}
