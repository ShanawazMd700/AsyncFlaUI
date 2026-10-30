using FlaUIAutomation.FSWLaunch;
using FlaUIAutomation.Pages;

namespace FlaUIAutomation.StepDefinitions;

[Binding]
public class DemoSteps
{
    private readonly HomePage _homePage; 
    private readonly AppLaunch _appLaunch;
    private readonly NOAHactions _noahActions;
    private readonly FittingScreen _fittingScreen;

    public DemoSteps()
    {
        _homePage = new HomePage();
        _appLaunch = new AppLaunch();
        _noahActions = new NOAHactions();
        _fittingScreen = new FittingScreen();
    }

    [When("Add or Select Patients is clicked")]
    public async Task WhenAddOrSelectPatientsIsClickedAsync()
    {
        await _homePage.ClickPatientDatabaseAsync();
    }

    [Given("Launch the application {string} in system")]
    public async Task GivenLaunchTheApplicationInSystemAsync(string app)
    {
        await _appLaunch.LaunchAppsAsync(app);
    }

    [When("The login password to NOAH {string} is entered")]
    public async Task WhenTheLoginPasswordToNOAHIsEnteredAsync(string p0)
    {
        await _noahActions.EnterPasswordAsync(p0);
    }


    [When("I click OK button in NOAH")]
    public async Task WhenIClickOKButtonInNOAHAsync()
    {
        await _noahActions.ClickOKButtonAsync();
    }
    [When("I add First name {string} and last Name {string}")]
    public async Task WhenIAddFirstNameAndLastNameAsync(string fname, string lname)
    {
        await _noahActions.AddPatientAsync(fname, lname);
    }
    [When("I Launch FSW {string}")]
    public async Task WhenILaunchFSWAsync(string fsw)
    {
        await _noahActions.LaunchFswAsync(fsw);
    }
    [When("I add patient in SA mode")]
    public async Task WhenIAddPatientInSAModeAsync()
    {
        await _homePage.AddNewpatientAsync("Demoooo", "JSDAWRG4229"); //await _homePage.AddNewpatientAsync(firstName, lastName);
    }

    [When("I select device {string} with the side {string}")]
    public async Task WhenISelectDeviceWithTheSideAsync(string device, string direction)
    {
        await _homePage.SelectDeviceWithFullnameAsync(device, direction);
    }
    [When("We navigate to the Fitting screen")]
    public async  Task WhenWeNavigateToTheFittingScreenAsync()
    {
        await _homePage.NavigateToFitScreeenAsync();
    }
    [When("We Save session")]
    public async Task WhenWeSaveSessionAsync()
    {
        await _fittingScreen.SaveSessionAsync();
    }
    [When("We Select the exixting patient {string}")]
    public async Task WhenWeSelectTheExixtingPatientAsync(string firstName)
    {
        await _homePage.SelectExixtingPatientAsync(firstName);
    }


}
