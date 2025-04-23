Feature: CreateClassFunctionality

  As a Teacher
  I want to be able to create a class with subjects
  So that I can manage students and lessons effectively

  Background:
    Given a Teacher user is logged in

  Scenario: Teacher creates a class with valid data
    When the Teacher sends a request to create a class with the following details:
      | className  | subjects             |
      | 5th Grade  | Math, English, Science |
    Then the response should indicate success
    And the class should be created with the correct details

  Scenario: Teacher creates a class with missing subjects
    When the Teacher sends a request to create a class with the following details:
      | className  | subjects |
      | 6th Grade  |          |
    Then the response should return a validation error
    And the class should not be created
  
  Scenario: Teacher creates a class with an already existing name
    Given a class named "5th Grade" already exists
    When the Teacher sends a request to create a class with the following details:
      | className  | subjects             |
      | 5th Grade  | Math, English, History |
    Then the response should indicate a conflict error
    And the class should not be created