Feature: UserLoginFunctionality

As a user 
I want to be able to login successfully with valid credentials
or getting validation when they are incorrect


Scenario: Admin logs in and retrieves a valid JWT token
    Given login data is prepared
    When an Admin user with valid username "admin5" and password "admin127"
    Then the response should return a valid JWT token
    And the token should contain Admin permissions

Scenario Outline: User logs in and retrieves a valid JWT token
    Given login data is prepared
    When "<role>" user with valid username - "<username>" and password - "<password>"
    Then the response should return a valid JWT token
    And the token should contain Admin permissions

    Examples:
      | role      | username  | password  |
      | admin     | admin5    | admin127  |
      | teacher   | teacher3  | teacher3  |
      | moderator | moderator | moderator |
      | parent    | parent1   | parent1   |

Scenario Outline: Login fails with missing fields
When a user submits a login request with missing or incorrect "<username>" or "<password>"
Then the response should return a validation error
And no JWT token should be returned

    Examples: 
    | username | password  |
    | admin5   |           |
    | admin5   | 123       |
    | 122      | moderator |
    | *        | parent1   |
    |          | parent1   |


