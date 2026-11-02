
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

namespace FlaUIAutomation.Drivers;

public static class AutomationManager
{
    private static UIA3Automation? _automation;
    private static Application? _application;
    private static Window? _mainWindow;

    public static UIA3Automation Automation
    {
        get => _automation
            ?? throw new InvalidOperationException(
                "FlaUI automation session has not been started.");
        set => _automation = value;
    }

    public static Application? Application
    {
        get => _application;
        set => _application = value;
    }

    public static Window? MainWindow
    {
        get => _mainWindow;
        set => _mainWindow = value;
    }

    public static AutomationElement Desktop => Automation.GetDesktop();

    public static AutomationElement SearchRoot => MainWindow ?? Desktop;

    public static bool IsStarted => _automation != null;

    public static void Clear()
    {
        _mainWindow = null;
        _application = null;
        _automation = null;
    }
}
