using System;
using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using BackEndAutomation.Utilities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Reqnroll;
using RestSharp;
using NUnit.Framework;
using System.Net;

namespace BackEndAutomation.Tests.BBDTests.Steps
{
    [Binding]
    public class ModeratorManagesClassesStepDefinitions
    {
        private RestCalls _restCalls = new RestCalls();
        private ResponseDataExtractors _dataExtractors = new ResponseDataExtractors();
        private ScenarioContext _scenarioContext;
        private ExtentTest _test;
        private RestResponse _response;
        private string _token;

        public ModeratorManagesClassesStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = _scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [Given("a Moderator user is logged in")]
        public void GivenAModeratorUserIsLoggedIn()
        {
            string username = "moderator";
            string password = "moderator";

            _response = _restCalls.SchoolApiLoginCall(username, password);

            _token = _dataExtractors.ExtractLoggedInUserToken(_response.Content, "access_token");
            _scenarioContext.Set(_token, "access_token");

            Logger.Log.Info("Moderator logged in successfully.");
        }

        [Given(@"two classes exist")]
        public void GivenTwoClassesExist()
        {
            var teacherResponse =  _restCalls.SchoolApiLoginCall("teacher3", "teacher3");
            var teacherToken = _dataExtractors.ExtractLoggedInUserToken(teacherResponse.Content, "access_token");
            _scenarioContext.Set(teacherToken, "access_token");

            string firstClassName = "FirstClass_" + DateTime.UtcNow.Ticks;
            List<string> firstClassSubjects = new List<string> { "Math", "English", "Physics" };
            var firstClassResponse = _restCalls.CreateClass(teacherToken, firstClassName, firstClassSubjects);

            var firstClassJson = JsonConvert.DeserializeObject<JObject>(firstClassResponse.Content);
            var firstClassId = firstClassJson["class_id"]?.ToString();

            if (string.IsNullOrEmpty(firstClassId))
            {
                throw new Exception("First class creation failed, class_id missing.");
            }

            _scenarioContext.Set(firstClassId, "FirstClassId");
            _scenarioContext.Set(firstClassSubjects, "FirstClassSubjects");

            string secondClassName = "SecondClass_" + DateTime.UtcNow.Ticks;
            List<string> secondClassSubjects = new List<string> { "Math", "English", "Physics" };
            var secondClassResponse = _restCalls.CreateClass(teacherToken, secondClassName, secondClassSubjects);

            var secondClassJson = JsonConvert.DeserializeObject<JObject>(secondClassResponse.Content);
            var secondClassId = secondClassJson["class_id"]?.ToString();

            if (string.IsNullOrEmpty(secondClassId))
            {
                throw new Exception("Second class creation failed, class_id missing.");
            }

            _scenarioContext.Set(secondClassId, "SecondClassId");
            _scenarioContext.Set(secondClassSubjects, "SecondClassSubjects");
        }

        [Given("a student is assigned to the first class")]
        public void GivenAStudentIsAssignedToTheFirstClass()
        {
            string token = _scenarioContext.Get<string>("access_token");

            string firstClassId = _scenarioContext.Get<string>("FirstClassId");

            string studentName = TestDataGenerator.GenerateRandomName();
            _scenarioContext.Set(studentName, "student_name");

            var addStudentResponse = _restCalls.AddStudentToClass(token, studentName, firstClassId);
            var responseJson = JsonConvert.DeserializeObject<JObject>(addStudentResponse.Content);
            string studentId = responseJson["student_id"]?.ToString();

            if (!string.IsNullOrEmpty(studentId))
            {
                _scenarioContext.Set(studentId, "student_id");
                Logger.Log.Info($"Student ID saved: {studentId}");
            }
            else
            {
                Logger.Log.Warn("Student ID was not found in the response.");
            }

            _scenarioContext.Set(addStudentResponse, "add_student_response");

            var subjects = _scenarioContext.Get<List<string>>("FirstClassSubjects");

            foreach (var subject in subjects)
            {
                int randomGrade = new Random().Next(2, 6); 
                var gradeResponse = _restCalls.AddOrUpdateGrade(token, studentId, subject, randomGrade);

                Logger.Log.Info($"Assigned grade {randomGrade} for {subject} to student {studentName}.");

                _scenarioContext.Set(gradeResponse, $"GradeResponse_{subject}");
                _scenarioContext.Set(randomGrade, $"Grade_{subject}");
            }
        }

        [When("the Moderator moves the student to the second class")]
        public void WhenTheModeratorMovesTheStudentToTheSecondClass()
        {
            _token = _dataExtractors.ExtractLoggedInUserToken(_response.Content, "access_token");
            var studentId = _scenarioContext.Get<string>("student_id");
            var secondClassId = _scenarioContext.Get<string>("SecondClassId");

            Logger.Log.Info($"Moving student {studentId} to class {secondClassId}");

            _response = _restCalls.MoveStudentToAnotherClass(_token, studentId, secondClassId);

            _scenarioContext.Set(_response, "move_student_response");

            Logger.Log.Info("Student move request sent successfully.");
        }

        [Then("the student should be assigned to the new class")]
        public void ThenTheStudentShouldBeAssignedToTheNewClass()
        {
            var moveStudentResponse = _scenarioContext.Get<RestResponse>("move_student_response");

            Assert.That(moveStudentResponse.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Student was not moved successfully.");

            var responseJson = JsonConvert.DeserializeObject<JObject>(moveStudentResponse.Content);
            string newClassId = responseJson["class_id"]?.ToString();
            string expectedClassId = _scenarioContext.Get<string>("SecondClassId");

            Logger.Log.Info($"Expected class ID: {expectedClassId}, Actual class ID after move: {newClassId}");

            Assert.That(newClassId, Is.EqualTo(expectedClassId), "Student was not assigned to the expected new class.");
        }

        [Given("an empty class created")]
        public void GivenAnEmptyClassCreated()
        {
            var teacherResponse = _restCalls.SchoolApiLoginCall("teacher3", "teacher3");
            var teacherToken = _dataExtractors.ExtractLoggedInUserToken(teacherResponse.Content, "access_token");
            _scenarioContext.Set(teacherToken, "access_token");

            string emptyClassName = "EmptyClass_" + DateTime.UtcNow.Ticks;
            List<string> emptyClassSubjects = new List<string> { "Math", "English", "Physics" };
            var emptyClassResponse = _restCalls.CreateClass(teacherToken, emptyClassName, emptyClassSubjects);

            var emptyClassJson = JsonConvert.DeserializeObject<JObject>(emptyClassResponse.Content);
            var emptyClassId = emptyClassJson["class_id"]?.ToString();

            if (string.IsNullOrEmpty(emptyClassId))
            {
                throw new Exception("Empty class creation failed, class_id missing.");
            }

            _scenarioContext.Set(emptyClassId, "EmptyClassId");
        }

        [When("the Moderator deletes the empty class")]
        public void WhenTheModeratorDeletesTheEmptyClass()
        {
            _token = _dataExtractors.ExtractLoggedInUserToken(_response.Content, "access_token");
            var emptyClassId = _scenarioContext.Get<string>("EmptyClassId");

            var deleteResponse = _restCalls.DeleteClass(_token, emptyClassId);

            _scenarioContext.Set(deleteResponse, "delete_class_response");

            Logger.Log.Info($"Sent delete request for class ID: {emptyClassId}");
        }

        [Then("the class should be deleted successfully")]
        public void ThenTheClassShouldBeDeletedSuccessfully()
        {
            var deleteResponse = _scenarioContext.Get<RestResponse>("delete_class_response");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Class was not deleted successfully.");
            Logger.Log.Info("Class deleted successfully.");
        }

        [Given("a class exists with students")]
        public void GivenAClassExistsWithStudents()
        {
            var teacherResponse = _restCalls.SchoolApiLoginCall("teacher3", "teacher3");
            var teacherToken = _dataExtractors.ExtractLoggedInUserToken(teacherResponse.Content, "access_token");
            _scenarioContext.Set(teacherToken, "teacher_token");

            string className = "NonEmptyClass_" + DateTime.UtcNow.Ticks;
            List<string> classSubjects = new List<string> { "Math", "Science", "History" };
            var classResponse = _restCalls.CreateClass(teacherToken, className, classSubjects);

            var classJson = JsonConvert.DeserializeObject<JObject>(classResponse.Content);
            var classId = classJson["class_id"]?.ToString();

            if (string.IsNullOrEmpty(classId))
            {
                throw new Exception("Class creation failed, class_id missing.");
            }

            _scenarioContext.Set(classId, "NonEmptyClassId");
            _scenarioContext.Set(classSubjects, "NonEmptyClassSubjects");

            string studentName = TestDataGenerator.GenerateRandomName();
            _scenarioContext.Set(studentName, "student_name");

            var addStudentResponse = _restCalls.AddStudentToClass(teacherToken, studentName, classId);
            var responseJson = JsonConvert.DeserializeObject<JObject>(addStudentResponse.Content);
            string studentId = responseJson["student_id"]?.ToString();

            if (!string.IsNullOrEmpty(studentId))
            {
                _scenarioContext.Set(studentId, "student_id");
                Logger.Log.Info($"Student added to class. Student ID: {studentId}");
            }
            else
            {
                Logger.Log.Warn("Student ID was not found in the response.");
            }
        }

        [When("the Moderator tries to delete the non-empty class")]
        public void WhenTheModeratorTriesToDeleteTheNon_EmptyClass()
        {
            _token = _scenarioContext.Get<string>("access_token");
            var nonEmptyClassId = _scenarioContext.Get<string>("NonEmptyClassId");

            var deleteResponse = _restCalls.DeleteClass(_token, nonEmptyClassId);

            _scenarioContext.Set(deleteResponse, "delete_class_response");

            Logger.Log.Info($"Tried to delete non-empty class with ID: {nonEmptyClassId}");
        }

        [Then("the class should not be deleted")]
        public void ThenTheClassShouldNotBeDeleted()
        {
            var deleteResponse = _scenarioContext.Get<RestResponse>("delete_class_response");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest)
                                                  .Or.EqualTo(HttpStatusCode.Conflict)
                                                  .Or.EqualTo(HttpStatusCode.Forbidden),
                        "Unexpected status code for deleting non-empty class.");
        }

        [Then("a meaningful error should be returned")]
        public void ThenAMeaningfulErrorShouldBeReturned()
        {
            var deleteResponse = _scenarioContext.Get<RestResponse>("delete_class_response");

            var responseJson = JsonConvert.DeserializeObject<JObject>(deleteResponse.Content);

            string errorMessage = responseJson["detail"]?.ToString();

            Logger.Log.Info($"Error message received: {errorMessage}");

            Assert.That(errorMessage, Is.Not.Null, "Expected a meaningful error message but got none.");
            Assert.That(errorMessage, Is.Not.Empty, "Expected a meaningful error message but got an empty one.");
        }
    }
}
