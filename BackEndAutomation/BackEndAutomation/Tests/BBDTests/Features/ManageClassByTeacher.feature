Feature: Manage class by Teacher

  As a Teacher
  I want to be able to create a class, add students, and manage grades
  So that I can maintain class data easily

Background:
	Given a Teacher user is logged in

Scenario: Teacher creates a class with valid data
	When the Teacher sends a request to create a class with the following details:
		| Subject |
		| Math    |
		| Science |
		| English |
	Then the response should indicate success
	And the class should be created with the correct details

Scenario: Teacher creates a class with missing subjects
	When the Teacher sends a request to create a class with the following details:
		| Subject |
		| Math    |
		| Science |
	Then the class creation response should return a validation error
	And the class should not be created
  
Scenario: Teacher creates a class with an already existing name
	When the Teacher sends a request to create a class with the following details:
		| Subject |
		| Math    |
		| Science |
		| English |
	And the Teacher tries to send request with the same Class name
	Then the response should indicate a conflict error

Scenario: Teacher successfully adds a student to a class
	Given a class with subjects exists
	When the Teacher sends a request to add a student
	Then the response should indicate the student was added successfully
	And the student should be enrolled in the class with inherited subjects

Scenario: Teacher tries to add a student without providing a name
	Given a class with subjects exists
	When the Teacher sends a request to add a student with an empty name and a valid class id
	Then the response should indicate a bad request error
	
Scenario: Teacher tries to add a student without providing a class id
	Given a class with subjects exists
	When the Teacher sends a request to add a student with a valid name and an empty class id
	Then the response should indicate a bad request error

Scenario: Teacher cannot add more than 20 students to a class
	Given the Teacher sends a request to create a new class for 20 students with the following:
		| Subject |
		| Math    |
		| Science |
		| English |
	And the class already has 20 students
	When the Teacher tries to add another student
	Then the class addition response should return a validation error