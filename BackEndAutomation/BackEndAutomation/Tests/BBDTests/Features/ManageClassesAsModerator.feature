Feature: Moderator manages classes
  As a Moderator
  I want to be able to move students and delete empty classes
  So that the school structure stays clean

  Background:
    Given a Moderator user is logged in

  Scenario: Moderator moves a student from one class to another
    Given two classes exist
    And a student is assigned to the first class
    When the Moderator moves the student to the second class
    Then the student should be assigned to the new class

  Scenario: Moderator deletes an empty class
    Given an empty class created
    When the Moderator deletes the empty class
    Then the class should be deleted successfully

  Scenario: Moderator cannot delete a class that is not empty
    Given a class exists with students
    When the Moderator tries to delete the non-empty class
    Then the class should not be deleted
    And a meaningful error should be returned