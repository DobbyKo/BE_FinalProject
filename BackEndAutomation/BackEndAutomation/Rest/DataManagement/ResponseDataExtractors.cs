using Newtonsoft.Json.Linq;

namespace BackEndAutomation.Rest.DataManagement
{
    public class ResponseDataExtractors
    {

        public string ExtractLoggedInUserToken(string jsonResponse, string jsonIdentfier = "token")
        {
            JObject jsonObject = JObject.Parse(jsonResponse);
            return jsonObject[jsonIdentfier]?.ToString();
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
    }
}
