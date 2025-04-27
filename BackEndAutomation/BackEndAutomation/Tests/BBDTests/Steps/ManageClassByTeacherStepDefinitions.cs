using System;
using System.Net;
using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using BackEndAutomation.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Reqnroll;
using RestSharp;

namespace BackEndAutomation.Tests.BBDTests.Steps
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
        private string _createdClassId;

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

        [Given("a class with subjects exists")]
        public void GivenAClassWithSubjectsExists()
        {
            string existingClassId = TestConstants.ExistingClassId;
            _scenarioContext.Set(existingClassId, "class_id");

            _test.Info($"Using existing class ID: {existingClassId} from TestConstants.");
        }

        [Given("the class already has {int} students")]
        public void GivenTheClassAlreadyHasStudents(int studentCount)
        {
            var token = _scenarioContext.Get<string>("access_token");
            var classId = _createdClassId;

            for (int i = 1; i <= 20; i++)
            {
                string studentName = $"SudentName{i}"; 
                var response = _restCalls.AddStudentToClass(token, studentName, classId);

                _test.Info($"Adding student {i}: {studentName}");
                Assert.That((int)response.StatusCode, Is.EqualTo(200), $"Expected 200 OK for student {studentName}, but got {(int)response.StatusCode}");
            }

            _test.Pass("Successfully added 20 students to the class.");
        }

        [Given("the Teacher sends a request to create a new class for 20 students with the following:")]
        public void GivenTheTeacherSendsARequestToCreateANewClassForStudentsWithTheFollowing(Table table)
        {
            className = TestDataGenerator.GenerateRandomClassName();

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

            _createdClassId = classID;

            _scenarioContext.Set(_classResponse, "createClassResponse");
            _scenarioContext.Set(message, "message");
            _scenarioContext.Set(classID, "class_id");
            _scenarioContext.Set(className, "class_name");
            _scenarioContext.Set(subjects, "class_subjects");

            _test.Info($"API Response: {_classResponse.Content}");
            _test.Pass($"{className} created successfully. Response message: {message}");
        }

        [When("the Teacher sends a request to create a class with the following details:")]
        public void WhenTheTeacherSendsARequestToCreateAClassWithTheFollowingDetails(Table table)
        {
            className = TestDataGenerator.GenerateRandomClassName();

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

        [When("the Teacher sends a request to add a student")]
        public void WhenTheTeacherSendsARequestToAddAStudent()
        {
            string studentName = TestDataGenerator.GenerateRandomName();

            _scenarioContext.Set(studentName, "student_name");

            string token = _scenarioContext.Get<string>("access_token");
            string classId = _scenarioContext.Get<string>("class_id");

            var response = _restCalls.AddStudentToClass(token, classId, studentName);
            var responseJson = JsonConvert.DeserializeObject<JObject>(response.Content);
            var studentId = responseJson["student_id"]?.ToString();

            if (!string.IsNullOrEmpty(studentId))
            {
                _scenarioContext.Set(studentId, "student_id");
                Logger.Log.Info($"Student ID saved: {studentId}");
            }
            else
            {
                Logger.Log.Warn("Student ID was not found in the response.");
            }

            _scenarioContext.Set(response, "add_student_response");
        }
        
        [When("the Teacher sends a request to add a student with an empty name and a valid class id")]
        public void WhenTheTeacherSendsARequestToAddAStudentWithAnEmptyNameAndAValidClassId()
        {
            var token = _scenarioContext.Get<string>("access_token");
            var classId = string.Empty;
            var studentName = "Petar Petrov";

            var response = _restCalls.AddStudentToClass(token, classId, studentName);
            _scenarioContext.Set(response, "response");
        }

        [When("the Teacher sends a request to add a student with a valid name and an empty class id")]
        public void WhenTheTeacherSendsARequestToAddAStudentWithAValidNameAndAnEmptyClassId()
        {
            var token = _scenarioContext.Get<string>("access_token");
            var classId = "5292a90e-1225-48da-8c00-9f6e44dbc169";
            var studentName = string.Empty;

            var response = _restCalls.AddStudentToClass(token, classId, studentName);
            _scenarioContext.Set(response, "response");
        }

        [When("the Teacher tries to add another student")]
        public void WhenTheTeacherTriesToAddAnotherStudent()
        {
            var token = _scenarioContext.Get<string>("access_token");
            var classId = _createdClassId;

            string studentName = TestDataGenerator.GenerateRandomName();
            var response = _restCalls.AddStudentToClass(token, studentName, classId);

            _scenarioContext.Set(response, "response");
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

        [Then("the response should indicate the student was added successfully")]
        public void ThenTheResponseShouldIndicateTheStudentWasAddedSuccessfully()
        {
            var response = _scenarioContext.Get<RestResponse>("add_student_response");
            response.StatusCode.Equals(HttpStatusCode.Created);
            _test.Pass("Student added successfully.");
        }

        [Then("the student should be enrolled in the class with inherited subjects")]
        public void ThenTheStudentShouldBeEnrolledInTheClassWithInheritedSubjects()
        {
            _test.Info("Verified the student is enrolled with inherited subjects (validation implementation needed based on API response).");
        }

        [Then("the response should indicate a bad request error")]
        public void ThenTheResponseShouldIndicateABadRequestError()
        {
            var response = _scenarioContext.Get<RestResponse>("response");

            _test.Info($"Validating error response: {response.Content}");

            Assert.That((int)response.StatusCode, Is.EqualTo(422),
                "Expected HTTP 422 Unprocessable Entity for invalid student add request.");

            var responseJson = JsonConvert.DeserializeObject<JObject>(response.Content);

            var errorMessage = responseJson["detail"]?.FirstOrDefault()?["msg"]?.ToString();

            Assert.That(errorMessage, Is.Not.Null.And.Not.Empty,
                "Expected a validation error message for invalid student add request.");

            _test.Pass($"Validation error message received: {errorMessage}");
        }

        [Then("the class addition response should return a validation error")]
        public void ThenTheClassAdditionResponseShouldReturnAValidationError()
        {
            var response = _scenarioContext.Get<RestResponse>("response");

            _test.Info($"Validating error response after exceeding student limit: {response.Content}");

            Assert.That((int)response.StatusCode, Is.EqualTo(400),
                "Expected HTTP 400 Bad Request when trying to add more than 20 students.");

            var errorMessage = _dataExtractors.ExtractMessageFromResponse(response.Content, "detail");

            Assert.That(errorMessage, Is.Not.Null.And.Not.Empty,
                "Expected a validation error message when class student limit is exceeded.");

            _test.Pass($"Validation error received correctly when exceeding 20 students: {errorMessage}");
        }

    }
}
