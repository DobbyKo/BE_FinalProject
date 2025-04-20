Feature: DemoUI

  @UI
  Scenario: Search for Selenium in Google
    Given I open the Google homepage
    When I search for "Selenium C#"
    Then the first result should contain "Selenium"