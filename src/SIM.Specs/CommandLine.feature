Feature: SIM Command Line
	In order to work better with Sitecore
	As a developer
	I want a command line to work with Sitecore instances

@SIMCMD
Scenario: Create instance
	Given No Sitecore intance named 'TestExample' exists
	When I create 'TestExample' with the command tool
	Then I can navigate to 'TestExample'
    Then I see the Sitecore Welcome page
    Then Delete 'TestExample'
