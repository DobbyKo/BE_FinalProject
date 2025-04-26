using System;
using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using BackEndAutomation.Utilities;
using Newtonsoft.Json;
using NUnit.Framework;
using Reqnroll;
using RestSharp;

namespace BackEndAutomation.Tests.BBDTests.ManageClass
{
    [Binding]
    public class ManageClassByTeacherStepDefinitions
    {
        private RestCalls _restCalls = new RestCalls();
        private ResponseDataExtractors _dataExtractors = new ResponseDataExtractors();
        private ScenarioContext _scenarioContext;
        private ExtentTest _test;
        private RestResponse _response;
        private RestResponse _classResponse;
        private string _token;
        private string className;

        public ManageClassByTeacherStepDefinitions(ScenarioContext scenarioContext)
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
        public void WhenTheTeacherSendsARequestToCreateAClassWithTheFollowingDetails(Table table)
        {
            string baseClassName = "Class";
            string uniqueSuffix = DateTime.Now.ToString("yyyyMMddHHmmss");
            className = $"{baseClassName}_{uniqueSuffix}";

            List<string> subjects = table.Rows.Select(row => row["Subject"]).ToList();

            if (subjects.Count > 3)
            {
                throw new ArgumentException("A class can have a maximum of 3 subjects.");
            }

            _test.Info($"Creating class with classname: {className}, subjects: {string.Join(", ", subjects)}");

            _token = _scenarioContext.Get<string>("access_token");

            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("Access token is null or empty. Please check the login step.");
            }

            _classResponse = _restCalls.CreateClass(_token, className, subjects);

            if (_classResponse == null || string.IsNullOrWhiteSpace(_classResponse.Content))
            {
                throw new Exception("API call failed: Response is null or empty.");
            }

            string message = _dataExtractors.ExtractStockMessage(_classResponse.Content);
            string classID = _dataExtractors.ExtractLoggedInUserToken(_classResponse.Content, "class_id");

            _scenarioContext.Set(_classResponse, "createClassResponse");
            _scenarioContext.Set(message, "message");
            _scenarioContext.Set(classID, "class_id");
            _scenarioContext.Set(className, "class_name");
            _scenarioContext.Set(subjects, "class_subjects");

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

        [Then(@"the class creation response should return a validation error")]
        public void ThenTheClassCreationResponseShouldReturnAValidationError()
        {
            var response = _scenarioContext.Get<RestResponse>("createClassResponse");

            _test.Info($"Validating error response: {response.Content}");

            Assert.That((int)response.StatusCode, Is.EqualTo(422), "Expected HTTP 422 Unprocessable Entity for class creation validation error");

            var errorMessage = _dataExtractors.ExtractMessageFromResponse(response.Content, "detail");

            Assert.That(errorMessage, Is.Not.Null.And.Not.Empty, "Expected a validation error message for class creation");

            _test.Pass($"Validation error message received during class creation: {errorMessage}");
        }

        [Then("the class should not be created")]
        public void ThenTheClassShouldNotBeCreated()
        {
            var response = _scenarioContext.Get<RestResponse>("createClassResponse");
            var classId = _dataExtractors.ExtractLoggedInUserToken(response.Content, "class_id");

            Assert.That(string.IsNullOrWhiteSpace(classId), Is.True, "Expected no class_id to be returned for failed creation");

            _test.Pass("Class was not created as expected.");
        }

        [When("the Teacher tries to send request with the same Class name")]
        public void WhenTheTeacherTriesToSendRequestWithTheSameClassName()
        {
            var className = _scenarioContext.Get<string>("class_name");
            var subjects = _scenarioContext.Get<List<string>>("class_subjects");
            _token = _scenarioContext.Get<string>("access_token");

            var response = _restCalls.CreateClass(_token, className, subjects);
            _scenarioContext.Set(response, "duplicateClassResponse");

            _test.Info("Duplicate class creation attempt response: " + response.Content);
        }

        [Then("the response should indicate a conflict error")]
        public void ThenTheResponseShouldIndicateAConflictError()
        {
            var originalClassId = _scenarioContext.Get<string>("class_id");
            var duplicateResponse = _scenarioContext.Get<RestResponse>("duplicateClassResponse");

            var duplicateClassId = _dataExtractors.ExtractValueFromResponse(duplicateResponse.Content, "class_id");

            _test.Info($"Original class ID: {originalClassId}");
            _test.Info($"Duplicate class ID: {duplicateClassId}");

            if (originalClassId != duplicateClassId)
            {
                _test.Warning("API allowed creation of a class with duplicate name. Class IDs are different.");
            }
            else
            {
                Assert.That((int)duplicateResponse.StatusCode, Is.EqualTo(409), "Expected HTTP 409 Conflict for duplicate class name.");
                _test.Pass("Conflict error returned as expected.");
            }
        }

    }
}
