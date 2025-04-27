using System;
using System.Net;
using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using BackEndAutomation.Utilities;
using Reqnroll;
using RestSharp;

namespace BackEndAutomation.Tests.BBDTests.Steps
{
    [Binding]
    public class ManageGradesByTeacherStepDefinitions
    {
        private RestCalls _restCalls = new RestCalls();
        private ResponseDataExtractors _dataExtractors = new ResponseDataExtractors();
        private ScenarioContext _scenarioContext;
        private ExtentTest _test;
        private RestResponse _response;
        private string _token;

        public ManageGradesByTeacherStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = _scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [Given("a Student exists in the class")]
        public void GivenAStudentExistsInTheClass()
        {
            _token = _scenarioContext.Get<string>("access_token");
            var studentName = TestDataGenerator.GenerateRandomName();
            string existingClassId = TestConstants.ExistingClassId;
            _scenarioContext.Set(existingClassId, "class_id");

            _test.Info($"Using existing class ID: {existingClassId} from TestConstants.");

            _response = _restCalls.AddStudentToClass(_token, studentName, existingClassId);

            var studentId = _dataExtractors.ExtractStudentId(_response.Content); 

            _scenarioContext.Set(studentId, "StudentId");
            _scenarioContext.Set(studentName, "StudentName");

            Logger.Log.Info($"Student created: Name={studentName}, ID={studentId}");
        }

        [When("the Teacher sends a request to add a grade for the student")]
        public void WhenTheTeacherSendsARequestToAddAGradeForTheStudent(DataTable table)
        {
            var data = table.Rows[0];
            var subject = data["Subject"];
            var gradeValue = data["Grade"];

            int grade;
            if (gradeValue.ToUpper() == "RANDOM")
            {
                var random = new Random();
                grade = random.Next(2, 7); 
            }
            else
            {
                grade = int.Parse(gradeValue);
            }

            _token = _scenarioContext.Get<string>("access_token");
            var studentId = _scenarioContext.Get<string>("StudentId");

            _response = _restCalls.AddOrUpdateGrade(_token, studentId, subject, grade);

            _scenarioContext.Set(_response, "LastResponse");
            _scenarioContext.Set(grade, "LastGrade");
        }

        [Then("the grade should be added successfully")]
        public void ThenTheGradeShouldBeAddedSuccessfully()
        {
            _response = _scenarioContext.Get<RestResponse>("LastResponse");

            _response.StatusCode.Equals(HttpStatusCode.OK);
            _response.Content.Contains("Grade added successfully");

            Logger.Log.Info("Grade was added successfully.");
 
        }

        [Given("a Student already has a grade in Math")]
        public void GivenAStudentAlreadyHasAGradeInMath()
        {
            _token = _scenarioContext.Get<string>("access_token");
            var studentName = TestDataGenerator.GenerateRandomName();
            string existingClassId = TestConstants.ExistingClassId;
            _scenarioContext.Set(existingClassId, "class_id");

            _test.Info($"Using existing class ID: {existingClassId} from TestConstants.");

            _response = _restCalls.AddStudentToClass(_token, studentName, existingClassId);

            var studentId = _dataExtractors.ExtractStudentId(_response.Content);

            _scenarioContext.Set(studentId, "StudentId");
            _scenarioContext.Set(studentName, "StudentName");

            Logger.Log.Info($"Student created: Name={studentName}, ID={studentId}");

            int initialGrade = 4;
            var addGradeResponse = _restCalls.AddOrUpdateGrade(_token, studentId, "Math", initialGrade);

            _scenarioContext.Set(addGradeResponse, "LastResponse");

            Logger.Log.Info("Initial grade '4' for Math added.");
        }

        [When("the Teacher sends a request to update the grade to {int}")]
        public void WhenTheTeacherSendsARequestToUpdateTheGradeTo(int p0)
        {
            _token = _scenarioContext.Get<string>("access_token");
            var studentId = _scenarioContext.Get<string>("StudentId");

            int updatedGrade = 6;
            _response = _restCalls.AddOrUpdateGrade(_token, studentId, "Math", updatedGrade);

            _scenarioContext.Set(_response, "LastResponse");
            _scenarioContext.Set(updatedGrade, "LastGrade");
        }

        [Then("the grade should be updated successfully")]
        public void ThenTheGradeShouldBeUpdatedSuccessfully()
        {
            _response = _scenarioContext.Get<RestResponse>("LastResponse");

            _response.StatusCode.Equals(HttpStatusCode.OK);
            _response.Content.Contains("Grade updated successfully");

            Logger.Log.Info("Grade was updated successfully.");
        }
    }
}
