Feature: Calculator

Simple calculator for adding two numbers

@trialsteps @windowsApp

Scenario: Launching FDTS
	Given Launch the application "fdts" in system


Scenario: Launching NOAH
	Given Launch the application "NOAH" in system
	When I click OK button in NOAH
	When I add First name "John" and last Name "Doe"
	When I Launch FSW "Smart Fit"


Scenario: Adding and Launching FSW
	Given Launch the application "Smart Fit" in system
	When Add or Select Patients is clicked
	When I add patient in SA mode
	When I select device "sn962-drw" with the side "right"
	When We navigate to the Fitting screen
	When We Save session


Scenario: Selecting the patient again
	Given Launch the application "Smart Fit" in system
	When Add or Select Patients is clicked
	When I add patient in SA mode
	When I select device "sn962-drw" with the side "right"
	When We navigate to the Fitting screen
	When We Save session
	When We Select the exixting patient "jhjh" 