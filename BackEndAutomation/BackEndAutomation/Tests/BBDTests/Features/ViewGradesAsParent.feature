Feature: Parent views their child's grades
  As a Parent
  I want to see only my child's grades
  So that I can monitor their academic performance

  Background:
    Given a Parent user is logged in

  Scenario: Parent retrieves grades for their child
    Given a Student exists with grades and Parent is associated with that student
    When the Parent sends a request to view the student's grades
    Then the Parent should receive only the grades for their child

Scenario: Parent cannot view grades of a non-associated student
    Given a Student exists that is not associated with the Parent
    When the Parent sends a request to view the student's grades
    Then the Parent should receive a forbidden error
