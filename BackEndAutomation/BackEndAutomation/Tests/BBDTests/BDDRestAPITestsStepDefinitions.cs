using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using BackEndAutomation.Utilities;
using NUnit.Framework;
using Reqnroll;
using RestSharp;

namespace BackEndAutomation.Tests.BBDTests
{
    [Binding]
    public class BDDRestAPITestsStepDefinitions
    {

        private RestCalls restCalls = new RestCalls();
        private ResponseDataExtractors extractResponseData = new ResponseDataExtractors();
        private RestResponse userLoginResponse, userProfileDetailsResponse, userFollowResponse, userUnfollowResponse;
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest _test;

        public BDDRestAPITestsStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [Given("login data is prepared")]
        [When("login data is prepared")]
        public void GivenLoginDataIsPrepared()
        {
            //Console.WriteLine("Login data is prepared");
            _test.Log(Status.Info, "Login data is prepared");
        }

        [When("execute login API call")]
        public void WhenExecuteLoginAPICall()
        {
            userLoginResponse = restCalls.LoginCall("http://161.35.202.130:3000", "dbsdhsh", "sdhshs");
            _test.Log(Status.Info, "Login call is executed");
        }

        [When("execute login API call with \"(.*)\" username and \"(.*)\" password")]
        public void WhenExecuteLoginAPICallWithUsernameAndPassword(string username, string password)
        {
            userLoginResponse = restCalls.LoginCall("http://161.35.202.130:3000", username, password);
            _test.Log(Status.Info, $@"Login call is executed with ""{username}"" username and ""{password}"" password");
        }

        [Then("user data for logged in user is returned")]
        public void ThenUserDataForLoggedInUserIsReturned()
        {
            if (userLoginResponse.StatusCode == System.Net.HttpStatusCode.Created)
            {
                _test.Log(Status.Info, "User is logged in: " + userLoginResponse.Content);
            }
            else
            {
                _test.Log(Status.Fail, "User is not logged in: " + userLoginResponse.Content);
                Assert.Fail("User is not logged in: " + userLoginResponse.Content);
            }
        }

        [Given("user is logged in with API call")]
        public void GivenUserIsLoggedInWithAPICall()
        {
            userLoginResponse = restCalls.LoginCall("http://161.35.202.130:3000", "dbsdhsh", "sdhshs");

            UtilitiesMethods.AssertEqual(
                System.Net.HttpStatusCode.Created,
                userLoginResponse.StatusCode,
                "User is not logged in: " + userLoginResponse.Content,
                _scenarioContext);

            _test.Log(Status.Info, "User is logged in: " + userLoginResponse.Content);
        }

        [When("execute get profile details API call")]
        public void WhenExecuteGetProfileDetailsAPICall()
        {
            string token = extractResponseData.ExtractLoggedInUserToken(userLoginResponse.Content);
            string userId = extractResponseData.ExtractUserId(userLoginResponse.Content).ToString();
            userProfileDetailsResponse = restCalls.GetUserPageInformationCall("http://161.35.202.130:3000", userId, token);
            _test.Log(Status.Info, "Call for get profile details is executed");
        }

        [Then("user profile details are returned")]
        public void ThenUserProfileDetailsAreReturned()
        {
            /*
             * OLD IMPLEMENTAION
            if (userProfileDetailsResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("User profile details are:");
                Console.WriteLine(userProfileDetailsResponse.Content);
            }
            else
            {
                Assert.Fail("Problem getting user details: " + userProfileDetailsResponse.Content);
            }
            */
            UtilitiesMethods.AssertEqual(
                System.Net.HttpStatusCode.OK,
                userProfileDetailsResponse.StatusCode,
                "Problem getting user details: " + userProfileDetailsResponse.Content,
                _scenarioContext);

            _test.Log(Status.Info, "User profile details are: " + userProfileDetailsResponse.Content); // log message into the report
        }

        [When("execute get follow user profile with API call")]
        public void WhenExecuteGetFollowUserProfileWithAPICall()
        {
            string token = extractResponseData.ExtractLoggedInUserToken(userLoginResponse.Content);
            string userId = extractResponseData.ExtractUserId(userLoginResponse.Content).ToString();
            userFollowResponse = restCalls.ToFollowUser("http://161.35.202.130:3000", "8335", token, true);
        }

        [Then("user profile is followed")]
        public void ThenUserProfileIsFollowed()
        {
            followValidation("followed");
        }

        [Then("user profile is unfollowed")]
        public void ThenUserProfileIsUnfollowed()
        {
            followValidation("unfollowed");
        }

        [When("execute get unfollow user profile with API call")]
        public void WhenExecuteGetUnfollowUserProfileWithAPICall()
        {
            string token = extractResponseData.ExtractLoggedInUserToken(userLoginResponse.Content);
            string userId = extractResponseData.ExtractUserId(userLoginResponse.Content).ToString();
            userUnfollowResponse = restCalls.ToFollowUser("http://161.35.202.130:3000", "8335", token, true);
        }

        private void followValidation(string toBeFollowed)
        {
            RestResponse resposeToBeValidated;
            if (toBeFollowed == "followed")
            {
                resposeToBeValidated = userFollowResponse;
            }
            else
            {
                resposeToBeValidated = userUnfollowResponse;
            }
            if (resposeToBeValidated.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //Console.WriteLine($"User is {toBeFollowed}: " + resposeToBeValidated.Content);
                _test.Log(Status.Info, $"User is {toBeFollowed}: " + resposeToBeValidated.Content);
            }
            else
            {
                Assert.Fail($"Problem {toBeFollowed} user" + resposeToBeValidated.Content);
            }
        }
    }
}