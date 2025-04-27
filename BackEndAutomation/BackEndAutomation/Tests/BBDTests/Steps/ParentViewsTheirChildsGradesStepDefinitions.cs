using System;
using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using BackEndAutomation.Utilities;
using NUnit.Framework;
using Reqnroll;
using RestSharp;

namespace BackEndAutomation.Tests.BBDTests.Steps
{
    [Binding]
    public class ParentViewsTheirChildsGradesStepDefinitions
    {
        private RestCalls _restCalls = new RestCalls();
        private ResponseDataExtractors _dataExtractors = new ResponseDataExtractors();
        private ScenarioContext _scenarioContext;
        private ExtentTest _test;
        private RestResponse _response;
        private string _token;
        private string _studentId;

        public ParentViewsTheirChildsGradesStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = _scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [Given("a Parent user is logged in")]
        public void GivenAParentUserIsLoggedIn()
        {
            string username = "parentDobby";
            string password = "parentDobby";

            _response = _restCalls.SchoolApiLoginCall(username, password);

            _token = _dataExtractors.ExtractLoggedInUserToken(_response.Content, "access_token");
            _scenarioContext.Set(_token, "access_token");

            Logger.Log.Info("Parent logged in successfully.");
        }

        [Given("a Student exists with grades and Parent is associated with that student")]
        public void GivenAStudentExistsWithGradesAndParentIsAssociatedWithThatStudent()
        {
            _studentId = "8950260d-202d-464f-907f-77581d517c6f";
            _scenarioContext.Set(_studentId, "student_id");

            Logger.Log.Info($"Student exists with grades. Student ID: {_studentId}");
        }


        [When("the Parent sends a request to view the student's grades")]
        public void WhenTheParentSendsARequestToViewTheStudentsGrades()
        {
            _token = _scenarioContext.Get<string>("access_token");
            string studentId = _scenarioContext.Get<string>("student_id");

            _response = _restCalls.ViewGradesAsParent(_token, studentId);
            _scenarioContext.Set(_response, "last_response");

            Logger.Log.Info("Parent requested to view the student's grades.");
        }

        [Then("the Parent should receive only the grades for their child")]
        public void ThenTheParentShouldReceiveOnlyTheGradesForTheirChild()
        {
            _response = _scenarioContext.Get<RestResponse>("last_response");

            _test.Info("Asserting that the parent sees only their child's grades.");

            Assert.That(_response.IsSuccessful, Is.True, "Response was not successful.");

            var content = _response.Content;

            Assert.That(content, Does.Contain("\"grades\":"), "The response does not contain a 'grades' field.");

            Logger.Log.Info("Grades field exists in response. Response content: " + content);
        }

        [Given("a Student exists that is not associated with the Parent")]
        public void GivenAStudentExistsThatIsNotAssociatedWithTheParent()
        {
            _studentId = "cdb78a41-d957-4ec5-8118-c91a4e60ef1c";
            _scenarioContext.Set(_studentId, "student_id");

            Logger.Log.Info($"Student exists but not associated with parent. Student ID: {_studentId}");
        }

        [Then("the Parent should receive a forbidden error")]
        public void ThenTheParentShouldReceiveAForbiddenError()
        {
            _response = _scenarioContext.Get<RestResponse>("last_response");

            _test.Info("Asserting that parent cannot view grades of a non-associated student.");

            Assert.That(_response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Forbidden),
                "Expected 403 Forbidden but got a different status code.");

            Assert.That(_response.Content, Does.Contain("You can't view this student's grades"),
                "The response content does not mention 'You can't view this student's grades'.");

            Logger.Log.Info("Correctly received 403 Forbidden when accessing unassociated student grades.");
        }

    }
}
