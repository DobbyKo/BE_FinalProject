using Newtonsoft.Json;
using System.Net;
using RestSharp;
using BackEndAutomation.Utilities;

namespace BackEndAutomation.Rest.Calls
{
    public class RestCalls
    {
        public RestResponse SchoolApiLoginCall(string username, string password)
        {
            RestClientOptions options = new RestClientOptions("https://schoolprojectapi.onrender.com")
            {
                Timeout = TimeSpan.FromSeconds(120),
            };

            RestClient client = new RestClient(options);
            RestRequest request = new RestRequest("/auth/login", Method.Post);

            request.AlwaysMultipartFormData = true;
            request.AddParameter("username", username);
            request.AddParameter("password", password);
            RestResponse response = client.Execute(request);

            Logger.Log.Info($"Login Response: {response.Content}");

            return response;
        }

        public string GetTokenFromResponse(RestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return responseContent?.token;
            }

            Logger.Log.Warn($"Login failed with status code: {response.StatusCode}. Response: {response.Content}");
            return null;
        }

        public RestResponse CreateClass(string token, string className, List<string> subjects)
        {
            if (subjects == null || subjects.Count == 0 || subjects.Count > 3)
                throw new ArgumentException("Class must have between 1 and 3 subjects.");

            var options = new RestClientOptions("https://schoolprojectapi.onrender.com")
            {
                Timeout = TimeSpan.FromSeconds(120),
            };

            var client = new RestClient(options);
            var request = new RestRequest("/classes/create", Method.Post);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");

            request.AddQueryParameter("class_name", className);

            for (int i = 0; i < subjects.Count; i++)
            {
                request.AddQueryParameter($"subject_{i + 1}", subjects[i]);
            }

            Logger.Log.Info($"Sending request with parameters: class_name={className}, subjects={string.Join(", ", subjects)}");

            RestResponse response = client.Execute(request);

            Logger.Log.Info($"Response Content: {response.Content}");

            return response;
        }
        public RestResponse AddStudentToClass(string token, string studentName, string classId)
        {
            var options = new RestClientOptions("https://schoolprojectapi.onrender.com")
            {
                Timeout = TimeSpan.FromSeconds(120),
            };

            var client = new RestClient(options);
            var request = new RestRequest("/classes/add_student", Method.Post);

            request.AddHeader("Authorization", $"Bearer {token}");

            if (!string.IsNullOrEmpty(studentName))
                request.AddQueryParameter("name", studentName);

            if (!string.IsNullOrEmpty(classId))
                request.AddQueryParameter("class_id", classId);

            Logger.Log.Info($"Sending request to add student: name={studentName ?? "MISSING"}, class_id={classId ?? "MISSING"}");

            var response = client.Execute(request);

            Logger.Log.Info($"Response Content: {response.Content}");

            return response;
        }

        public RestResponse AddOrUpdateGrade(string token, string studentId, string subjectName, int grade)
        {
            var options = new RestClientOptions("https://schoolprojectapi.onrender.com")
            {
                Timeout = TimeSpan.FromSeconds(120),
            };

            var client = new RestClient(options);
            var request = new RestRequest("/grades/add", Method.Put);

            request.AddHeader("Authorization", $"Bearer {token}");

            if (!string.IsNullOrEmpty(studentId))
                request.AddQueryParameter("student_id", studentId);

            if (!string.IsNullOrEmpty(subjectName))
                request.AddQueryParameter("subject", subjectName);

            request.AddQueryParameter("grade", grade.ToString());

            Logger.Log.Info($"Sending request to add/update grade: student_id={studentId ?? "MISSING"}, subject={subjectName ?? "MISSING"}, grade={grade}");

            var response = client.Execute(request);

            Logger.Log.Info($"Response Content: {response.Content}");

            return response;
        }

        public RestResponse ViewGradesAsParent(string token, string studentId)
        {
            var options = new RestClientOptions("https://schoolprojectapi.onrender.com")
            {
                Timeout = TimeSpan.FromSeconds(120),
            };

            var client = new RestClient(options);
            var request = new RestRequest($"/grades/student/{studentId}", Method.Get);

            request.AddHeader("Authorization", $"Bearer {token}");

            Logger.Log.Info($"Sending request to view grades for student_id={studentId ?? "MISSING"}");

            var response = client.Execute(request);

            Logger.Log.Info($"Response Content: {response.Content}");

            return response;
        }

        public RestResponse MoveStudentToAnotherClass(string token, string studentId, string targetClassId)
        {
            var options = new RestClientOptions("https://schoolprojectapi.onrender.com")
            {
                Timeout = TimeSpan.FromSeconds(120),
            };
            var client = new RestClient(options);

            var request = new RestRequest("/classes/move_student", Method.Put);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddQueryParameter("student_id", studentId);
            request.AddQueryParameter("target_class_id", targetClassId);

            Logger.Log.Info($"Sending request to move student {studentId} to class {targetClassId}");

            RestResponse response = client.Execute(request);

            Logger.Log.Info($"Move student response status: {response.StatusCode}");

            return response;
        }
        public  RestResponse DeleteClass(string token, string classId)
        {
            var options = new RestClientOptions("https://schoolprojectapi.onrender.com")
            {
                Timeout = TimeSpan.FromSeconds(120),
            };
            var client = new RestClient(options);

            var request = new RestRequest($"/classes/delete_class_if_empty/{classId}", Method.Delete);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute(request);

            Logger.Log.Info($"Delete class response status: {response.StatusCode}");
            Logger.Log.Debug($"Delete class response content: {response.Content}");

            return response;
        }

    }
}
