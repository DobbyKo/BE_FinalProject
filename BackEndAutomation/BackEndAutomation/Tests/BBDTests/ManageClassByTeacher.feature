Feature: Manage class by Teacher

  As a Teacher
  I want to be able to create a class, add students, and manage grades
  So that I can maintain class data easily

  Background:
    Given a Teacher user is logged in

  Scenario: Teacher creates a class with valid data
    When the Teacher sends a request to create a class with the following details:
        | Subject   |
        | Math      |
        | Science   |
        | English   |
    Then the response should indicate success
    And the class should be created with the correct details

  Scenario: Teacher creates a class with missing subjects
    When the Teacher sends a request to create a class with the following details:
        | Subject   |
        | Math      |
        | Science   |
    Then the class creation response should return a validation error
    And the class should not be created
  
  Scenario: Teacher creates a class with an already existing name
    When the Teacher sends a request to create a class with the following details:
        | Subject   |
        | Math      |
        | Science   |
        | English   |
    And the Teacher tries to send request with the same Class name
    Then the response should indicate a conflict error