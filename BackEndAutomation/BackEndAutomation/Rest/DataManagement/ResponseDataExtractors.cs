using BackEndAutomation.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BackEndAutomation.Rest.DataManagement
{
    public class ResponseDataExtractors
    {
        public string ExtractLoggedInUserToken(string jsonResponse, string jsonIdentifier = "token")
        {
            Logger.Log.Info($"Raw JSON response: {jsonResponse}");
            ValidateJsonResponse(jsonResponse);

            try
            {
                JObject jsonObject = JObject.Parse(jsonResponse);
                string token = jsonObject[jsonIdentifier]?.ToString();

                if (string.IsNullOrEmpty(token))
                {
                    Logger.Log.Error($"Token '{jsonIdentifier}' not found in JSON response!");
                }

                return token;
            }
            catch (JsonReaderException ex)
            {
                HandleJsonParsingError(jsonResponse, ex);
                throw;
            }
        }

        public string ExtractStockMessage(string jsonResponse)
        {
            Logger.Log.Info($"Raw JSON response for extracting stock message: {jsonResponse}");
            ValidateJsonResponse(jsonResponse);

            try
            {
                JObject jsonObject = JObject.Parse(jsonResponse);
                string message = jsonObject["message"]?.ToString();

                if (string.IsNullOrEmpty(message))
                {
                    Logger.Log.Error("Message field not found in JSON response!");
                }

                return message;
            }
            catch (JsonReaderException ex)
            {
                HandleJsonParsingError(jsonResponse, ex);
                throw;
            }
        }

        public string ExtractMessageFromResponse(string jsonResponse, string jsonIdentifier = "message")
        {
            Logger.Log.Info($"Raw JSON response: {jsonResponse}");
            ValidateJsonResponse(jsonResponse);

            try
            {
                JObject jsonObject = JObject.Parse(jsonResponse);
                string message = jsonObject[jsonIdentifier]?.ToString();

                if (string.IsNullOrEmpty(message))
                {
                    Logger.Log.Error($"Message '{jsonIdentifier}' not found in response JSON!");
                }

                return message;
            }
            catch (JsonReaderException ex)
            {
                HandleJsonParsingError(jsonResponse, ex);
                throw;
            }
        }

        public string ExtractValueFromResponse(string jsonResponse, string key)
        {
            Logger.Log.Info($"Raw JSON response: {jsonResponse}");
            ValidateJsonResponse(jsonResponse);

            try
            {
                JObject jsonObject = JObject.Parse(jsonResponse);
                string value = jsonObject[key]?.ToString();

                if (string.IsNullOrEmpty(value))
                {
                    Logger.Log.Error($"Key '{key}' not found in response JSON!");
                }

                return value;
            }
            catch (JsonReaderException ex)
            {
                HandleJsonParsingError(jsonResponse, ex);
                throw;
            }
        }

        public string ExtractStudentId(string jsonResponse)
        {
            Logger.Log.Info($"Raw JSON response for extracting student ID: {jsonResponse}");
            ValidateJsonResponse(jsonResponse);

            try
            {
                JObject jsonObject = JObject.Parse(jsonResponse);
                string studentId = jsonObject["student_id"]?.ToString();

                if (string.IsNullOrEmpty(studentId))
                {
                    Logger.Log.Error("Student ID not found in response JSON!");
                }

                return studentId;
            }
            catch (JsonReaderException ex)
            {
                HandleJsonParsingError(jsonResponse, ex);
                throw;
            }
        }

        private void ValidateJsonResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                Logger.Log.Error("JSON response is empty or null!");
                throw new ArgumentException("JSON response is empty or null.");
            }
        }

        private void HandleJsonParsingError(string jsonResponse, JsonReaderException ex)
        {
            Logger.Log.Error($"Failed to parse JSON. Response was: {jsonResponse}");
            throw new Exception("Invalid JSON response received from server.", ex);
        }
    }
}
