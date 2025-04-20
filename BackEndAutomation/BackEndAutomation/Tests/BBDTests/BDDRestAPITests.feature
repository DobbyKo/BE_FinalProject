Feature: BDDRestAPITests

Test API calls with RestSharp and BDD tests

@login
Scenario: Login with API call
	When login data is prepared
	When execute login API call
	Then user data for logged in user is returned

@login
Scenario: Login with API call with parametrized steps
	Given login data is prepared
	When execute login API call with "dbsdhsh" username and "sdhshs" password
	Then user data for logged in user is returned

@login
Scenario Outline: Login with API call demo multiple parameters
	Given login data is prepared
	When execute login API call with "<username>" username and "<password>" password
	Then user data for logged in user is returned
	Examples: 
	| username      | password        |
	| dbsdhsh       | sdhshs          |
	| p0li0m        | TGdd7EDby83jdAC |
	| wrongusername | asfasfsafasf    |

@profiledetails @login
Scenario: Get Profile details with API call
	Given user is logged in with API call
	When execute get profile details API call
	Then user profile details are returned

@login
Scenario: Follow User Profile with API call
	Given user is logged in with API call
	When execute get follow user profile with API call
	Then user profile is followed

@login
Scenario: Unfollow User Profile with API call
	Given user is logged in with API call
	When execute get follow user profile with API call
	And execute get unfollow user profile with API call
	Then user profile is unfollowed