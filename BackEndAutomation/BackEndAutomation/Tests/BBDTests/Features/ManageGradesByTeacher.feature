Feature: ManageGradesByTeacher

 As a Teacher
  I want to add or update a grade for a student
  So that I can evaluate their performance

Background:
	Given a Teacher user is logged in

Scenario: Teacher adds a grade for a student
    Given a Student exists in the class
    When the Teacher sends a request to add a grade for the student
      | Subject | Grade |
      | Math    | 5     |
    Then the grade should be added successfully

Scenario: Teacher updates a grade for a student
    Given a Student already has a grade in Math
    When the Teacher sends a request to update the grade to 6
    Then the grade should be updated successfully
