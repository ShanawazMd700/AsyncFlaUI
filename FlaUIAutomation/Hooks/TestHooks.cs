using FlaUIAutomation.Drivers;

namespace FlaUIAutomation.Hooks;

[Binding]
public sealed class TestHooks
{
    [BeforeScenario]
    public void BeforeScenario()
    {
        SessionManager.StartSession();
    }

    [AfterScenario]
    public async Task AfterScenarioAsync()
    {
        await Task.Delay(9000);
        SessionManager.StopSession();
    }
}
