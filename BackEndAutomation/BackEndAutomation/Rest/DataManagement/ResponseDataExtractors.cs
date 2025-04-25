using BackEndAutomation.Utilities;
using Newtonsoft.Json.Linq;

namespace BackEndAutomation.Rest.DataManagement
{
    public class ResponseDataExtractors
    {

        public string ExtractLoggedInUserToken(string jsonResponse, string jsonIdentifier = "token")
        {
            Logger.Log.Info($"Raw JSON response: {jsonResponse}");

            JObject jsonObject = JObject.Parse(jsonResponse);
            string token = jsonObject[jsonIdentifier]?.ToString();

            if (string.IsNullOrEmpty(token))
            {
                Logger.Log.Error("Access token not found in response JSON!");
            }

            return token;
        }

        public int ExtractUserId(string jsonResponse)
        {
            var jsonObject = JObject.Parse(jsonResponse);
            return jsonObject["user"]?["id"]?.Value<int>() ?? 0;
        }

        public string ExtractStockMessage(string jsonResponse)
        {
            JObject jsonObject = JObject.Parse(jsonResponse);
            return jsonObject["message"]?.ToString();

        }

        public string ExtractMessageFromResponse(string jsonResponse, string jsonIdentifier = "message")
        {
            Logger.Log.Info($"Raw JSON response: {jsonResponse}");

            JObject jsonObject = JObject.Parse(jsonResponse);
            string message = jsonObject[jsonIdentifier]?.ToString();

            if (string.IsNullOrEmpty(message))
            {
                Logger.Log.Error("Message not found in response JSON!");
            }

            return message;
        }
    }
}
