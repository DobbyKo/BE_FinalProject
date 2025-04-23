using System;
using AventStack.ExtentReports;
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
        private string _token;

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

            string token = _restCalls.GetTokenFromResponse(_response);
            _scenarioContext.Set(token, "access_token");

            Logger.Log.Info("Teacher logged in successfully.");
        }

        [When("the Teacher sends a request to create a class with the following details:")]
        public void WhenTheTeacherSendsARequestToCreateAClassWithTheFollowingDetails(DataTable dataTable)
        {
            throw new PendingStepException();
        }

        [Then("the response should indicate success")]
        public void ThenTheResponseShouldIndicateSuccess()
        {
            throw new PendingStepException();
        }

        [Then("the class should be created with the correct details")]
        public void ThenTheClassShouldBeCreatedWithTheCorrectDetails()
        {
            throw new PendingStepException();
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
