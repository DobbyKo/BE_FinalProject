using System;
using AventStack.ExtentReports;
using BackEndAutomation.Models;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using BackEndAutomation.Utilities;
using Newtonsoft.Json;
using Reqnroll;
using RestSharp;

namespace BackEndAutomation.Tests.BBDTests
{
    [Binding]
    public class CreateClassFunctionalityStepDefinitions
    {
        private RestCalls _restCalls = new RestCalls();
        private ResponseDataExtractors _dataExtractors = new ResponseDataExtractors();
        private ScenarioContext _scenarioContext;
        private ExtentTest _test;
        private RestResponse _response;
        private RestResponse _classResponse;
        private string _token;
        private string className;

        public CreateClassFunctionalityStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = _scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [Given("a Teacher user is logged in")]
        public void GivenATeacherUserIsLoggedIn()
        {
            string username = "teacher3";
            string password = "teacher3";

            _response = _restCalls.SchoolApiLoginCall(username, password);

            _token = _dataExtractors.ExtractLoggedInUserToken(_response.Content, "access_token");
            _scenarioContext.Set(_token, "access_token");

            Logger.Log.Info("Teacher logged in successfully.");
        }

        [When("the Teacher sends a request to create a class with the following details:")]
        public void WhenTheTeacherSendsARequestToCreateAClassWithTheFollowingDetails()
        {
            string baseClassName = "Class";
            string uniqueSuffix = DateTime.Now.ToString("yyyyMMddHHmmss");
            className = $"{baseClassName}_{uniqueSuffix}";

            string subject1 = "Math";
            string subject2 = "Science";
            string subject3 = "English";

            _test.Info($"Creating class with classname: {className}, subject1={subject1}, subject2={subject2}, subject3={subject3}");

            string token = _scenarioContext.Get<string>("access_token");

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Access token is null or empty. Please check the login step.");
            }

            _classResponse = _restCalls.CreateClass(token, className, subject1, subject2, subject3);

            if (_classResponse == null || string.IsNullOrWhiteSpace(_classResponse.Content))
            {
                throw new Exception("API call failed: Response is null or empty.");
            }

            _scenarioContext.Set(_classResponse, "createClassResponse");

            string message = _dataExtractors.ExtractStockMessage(_classResponse.Content);
            string classID = _dataExtractors.ExtractLoggedInUserToken(_classResponse.Content, "class_id");

            _scenarioContext.Add("message", message);
            _scenarioContext.Add("class_id", classID);
            _scenarioContext.Add("class_name", className);

            Console.WriteLine($"Response Content: {_classResponse.Content}");
            _test.Info($"API Response: {_classResponse.Content}");

            _test.Pass($"{className} created successfully. Response message: {message}");
        }

        [Then("the response should indicate success")]
        public void ThenTheResponseShouldIndicateSuccess()
        {
            if (!_scenarioContext.TryGetValue("createClassResponse", out RestResponse _response))
            {
                Logger.Log.Error("The key 'createClassResponse' was not found in ScenarioContext.");
                throw new KeyNotFoundException("The key 'createClassResponse' was not found in ScenarioContext.");
            }

            UtilitiesMethods.AssertEqual(200, (int)_response.StatusCode, "Expected status code 200 OK", _scenarioContext);

            Logger.Log.Info($"Response validated successfully. Content: {_response.Content}");
        }

        [Then("the class should be created with the correct details")]
        public void ThenTheClassShouldBeCreatedWithTheCorrectDetails()
        {
            string expectedMessage = "Class created";
            string actualMessage = _dataExtractors.ExtractMessageFromResponse(_classResponse.Content);

            UtilitiesMethods.AssertEqual(expectedMessage, actualMessage, "Creation message does not match.", _scenarioContext);
        }

        [Then("the class should not be created")]
        public void ThenTheClassShouldNotBeCreated()
        {
            throw new PendingStepException();
        }

        [Given("a class named {string} already exists")]
        public void GivenAClassNamedAlreadyExists(string p0)
        {
            throw new PendingStepException();
        }

        [Then("the response should indicate a conflict error")]
        public void ThenTheResponseShouldIndicateAConflictError()
        {
            throw new PendingStepException();
        }
    }
}
