using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

namespace FlaUIAutomation.Drivers;

public static class AutomationManager
{
    private static readonly AsyncLocal<UIA3Automation?> CurrentAutomation = new();
    private static readonly AsyncLocal<Application?> CurrentApplication = new();
    private static readonly AsyncLocal<Window?> CurrentMainWindow = new();

    public static UIA3Automation Automation
    {
        get => CurrentAutomation.Value
            ?? throw new InvalidOperationException("FlaUI automation session has not been started.");
        set => CurrentAutomation.Value = value;
    }

    public static Application? Application
    {
        get => CurrentApplication.Value;
        set => CurrentApplication.Value = value;
    }

    public static Window? MainWindow
    {
        get => CurrentMainWindow.Value;
        set => CurrentMainWindow.Value = value;
    }

    public static AutomationElement Desktop => Automation.GetDesktop();

    public static AutomationElement SearchRoot => MainWindow ?? Desktop;

    public static bool IsStarted => CurrentAutomation.Value is not null;

    public static void Clear()
    {
        CurrentMainWindow.Value = null;
        CurrentApplication.Value = null;
        CurrentAutomation.Value = null;
    }
}
