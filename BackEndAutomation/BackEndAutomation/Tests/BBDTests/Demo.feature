Feature: Demo

A short summary of the feature

@tag1
Scenario: 2 My cal scenario
	Given open the calculator
	When click 8 number
		And click + action
		And click 6 number
		And click + action
		And click 6 number
		And click + action
		And click 6 number
		And click + action
		And click 6 number
		And click + action
		And click 6 number
		And click + action
		And click 6 number
		And click + action
		And click 2 number
		And click = action
	Then the calculator shows "14"

	@tag1
Scenario: 1 My cal scenario
	Given open the calculator
	When click 8 number
		And click + action
		And click 6 number
		And click + action
		And click 6 number
		And click + action
		And click 6 number
		And click + action
		And click 6 number
		And click + action
		And click 6 number
		And click + action
		And click 6 number
		And click + action
		And click 2 number
		And click = action
	Then the calculator shows "46"

	@tag1 @UI
Scenario Outline: 3 My cal scenario
	Given open the calculator
	When click <number1> number
		And click + action
		And click <number2> number
		And click = action
	Then the calculator shows "<sum>", or show the "<errorMessage>" error message
	Examples: 
	| number1 | number2 | sum | errorMessage                                                               |
	| 8       | 6       | 14  | "blahgdajgb"                                                               |
	| 7       | 7       | 14  | "sagsafgagag"                                                              |
	| 7       | 6       | 13  | "sgesgds"                                                                  |
	| 5       | 6       | 11  | gssdgsdgsdg                                                                |
	| 6       | 6       | 14  | The expected sum (14) is different than the actual sum REPLACEWITHACTUALSUM and bug needs to be open |