using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System;
using System.Linq;
namespace FlaUIAutomation.Drivers;
public static class SessionManager
{
    public static void StartSession()
    {
        if (AutomationManager.IsStarted)
        {
            return;
        }

        AutomationManager.Automation = new UIA3Automation();
    }

    public static async Task AttachApplicationAsync(Application application, TimeSpan? timeout = null)
    {
        StartSession();

        AutomationManager.Application = application;
        AutomationManager.MainWindow = await WaitForMainWindowAsync(application, timeout ?? TimeSpan.FromSeconds(30));
    }
    public static async Task ReattachMainWindowAsync(TimeSpan? timeout = null)
    {
        if (AutomationManager.Application == null)
        {
            throw new InvalidOperationException(
                "Application is not attached.");
        }

        AutomationManager.MainWindow =
            await WaitForMainWindowAsync(
                AutomationManager.Application,
                timeout ?? TimeSpan.FromSeconds(30));

       // Console.WriteLine(
            //$"Re-attached Window: " +
            //$"Title='{AutomationManager.MainWindow.Title}' | " +
            //$"AutomationId='{AutomationManager.MainWindow.AutomationId}'");
    }

    public static void StopSession()
    {
        try
        {
            AutomationManager.MainWindow = null;
            AutomationManager.Application?.Close();
        }
        finally
        {
            AutomationManager.Application?.Dispose();
            AutomationManager.Automation.Dispose();
            AutomationManager.Clear();
        }
    }

    private static async Task<Window> WaitForMainWindowAsync(
    Application application,
    TimeSpan timeout)
    {
        var endTime = DateTime.UtcNow + timeout;

        while (DateTime.UtcNow < endTime)
        {
            var windows = application.GetAllTopLevelWindows(
                AutomationManager.Automation);

            //Console.WriteLine(
            //    $"Found {windows.Length} top-level windows");

            foreach (var w in windows)
            {
                //Console.WriteLine(
                //    $"Window Title='{w.Title}'");
            }

            var smartFitWindow = windows.FirstOrDefault(
                w => !string.IsNullOrWhiteSpace(w.Title));

            if (smartFitWindow != null)
            {
                //Console.WriteLine(
                //    $"Selected Window='{smartFitWindow.Title}'");

                return smartFitWindow;
            }

            await Task.Delay(1000);
        }

        throw new TimeoutException(
            $"Main window was not available within {timeout.TotalSeconds:n0} seconds.");
    }
    public static async Task<Window> AttachToWindowAsync(
    Func<Window, bool> condition,
    TimeSpan? timeout = null)
    {
        if (AutomationManager.Application == null)
        {
            throw new InvalidOperationException(
                "Application is not attached.");
        }

        var waitTime = timeout ?? TimeSpan.FromSeconds(30);
        var endTime = DateTime.UtcNow + waitTime;

        while (DateTime.UtcNow < endTime)
        {
            var windows =
                AutomationManager.Application.GetAllTopLevelWindows(
                    AutomationManager.Automation);

            foreach (var window in windows)
            {
                try
                {
                    if (condition(window))
                    {
                        AutomationManager.MainWindow = window;

                        //Console.WriteLine(
                        //    $"Attached to window: " +
                        //    $"Title='{window.Title}', " +
                        //    $"AutomationId='{window.AutomationId}'");

                        return window;
                    }
                }
                catch
                {
                    // Window may disappear while enumerating
                }
            }

            await Task.Delay(500);
        }

        throw new TimeoutException(
            $"Could not find matching window within " +
            $"{waitTime.TotalSeconds:n0} seconds.");
    }

    public static Task<Window> AttachToWindowByTitleAsync(
    string title,
    TimeSpan? timeout = null)
    {
        return AttachToWindowAsync(
            w => string.Equals(
                w.Title,
                title,
                StringComparison.OrdinalIgnoreCase),
            timeout);
    }

    public static async Task SwitchToWindowAsync(
        string windowName,
    TimeSpan? timeout = null)
    {
        if (string.IsNullOrWhiteSpace(windowName))
        {
            throw new ArgumentException(
                "Window name cannot be empty.",
                nameof(windowName));
        }

        if (AutomationManager.Automation == null)
        {
            throw new InvalidOperationException(
                "UI Automation session has not started.");
        }

        var waitTime = timeout ?? TimeSpan.FromSeconds(15);
        var endTime = DateTime.UtcNow + waitTime;

        Console.WriteLine(
            $"Searching desktop for window: '{windowName}'");

        while (DateTime.UtcNow < endTime)
        {
            var desktop = AutomationManager.Automation.GetDesktop();

            // Enumerate immediate desktop children, including top-level windows.
            var windows = desktop.FindAllChildren(
                cf => cf.ByControlType(
                    FlaUI.Core.Definitions.ControlType.Window));

            foreach (var element in windows)
            {
                try
                {
                    Console.WriteLine(
                        $"Desktop window: Name='{element.Name}', " +
                        $"AutomationId='{element.AutomationId}'");

                    if (string.Equals(
                        element.Name?.Trim(),
                        windowName.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        var window = element.AsWindow();

                        AutomationManager.MainWindow = window;
                        window.Focus();

                        Console.WriteLine(
                            $"Switched to window: '{window.Title}'");

                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Could not inspect a desktop element: {ex.Message}");
                }
            }

            await Task.Delay(500);
        }

        throw new TimeoutException(
            $"Desktop window '{windowName}' was not found " +
            $"within {waitTime.TotalSeconds} seconds.");
    }

    public static void InspectAudiogramGrid()
    {
        var window = AutomationManager.MainWindow
            ?? throw new InvalidOperationException(
                "Audiogram Module is not attached.");

        Console.WriteLine(
            $"Inspecting window: '{window.Title}'");

        var elements = window.FindAllDescendants();

        Console.WriteLine(
            $"Total descendant elements: {elements.Length}");

        foreach (var element in elements)
        {
            string name = "";
            string automationId = "";
            string controlType = "";

            try { name = element.Name ?? ""; } catch { }
            try { automationId = element.AutomationId ?? ""; } catch { }
            try { controlType = element.ControlType.ToString(); } catch { }

            if (name.Contains(
                    "TestValuesViewModel",
                    StringComparison.OrdinalIgnoreCase)
                || name.Contains("theGrid",
                    StringComparison.OrdinalIgnoreCase)
                || controlType.Contains("Grid",
                    StringComparison.OrdinalIgnoreCase)
                || controlType.Contains("DataItem",
                    StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    $"Name='{name}' | " +
                    $"AutomationId='{automationId}' | " +
                    $"ControlType='{controlType}'");
            }
        }
    }
    public static async Task<AutomationElement>
        SwitchToWindowContainingAutomationIdAsync(
            string automationId,
            TimeSpan? timeout = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(automationId);

        var automation = AutomationManager.Automation
            ?? throw new InvalidOperationException(
                "UI Automation is not initialized.");

        var waitTime = timeout ?? TimeSpan.FromSeconds(60);
        var endTime = DateTime.UtcNow.Add(waitTime);

        while (DateTime.UtcNow < endTime)
        {
            var desktop = automation.GetDesktop();

            var windows = desktop.FindAllChildren(
                cf => cf.ByControlType(ControlType.Window));

            foreach (var windowElement in windows)
            {
                try
                {
                    var element = windowElement.FindFirstDescendant(
                        cf => cf.ByAutomationId(automationId));

                    if (element != null)
                    {
                        var window = windowElement.AsWindow();

                        AutomationManager.MainWindow = window;
                        window.Focus();
                        Console.WriteLine(
                         $"Application attached: {AutomationManager.Application != null}");

                        Console.WriteLine(
                            $"MainWindow assigned: {AutomationManager.MainWindow != null}");
                        Console.WriteLine(
                            $"Switched to window '{window.Name}' " +
                            $"containing AutomationId '{automationId}'.");

                        return element;
                    }
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    // The window may have closed or become unavailable.
                }
            }

            await Task.Delay(500);
        }

        throw new TimeoutException(
            $"Could not find AutomationId '{automationId}' " +
            $"within {waitTime.TotalSeconds} seconds.");
    }
    public static async Task CloseWindowAsync(
        string windowName,
    TimeSpan? timeout = null)
    {
        if (string.IsNullOrWhiteSpace(windowName))
        {
            throw new ArgumentException(
                "Window name cannot be empty.",
                nameof(windowName));
        }

        if (AutomationManager.Automation == null)
        {
            throw new InvalidOperationException(
                "UI Automation session has not started.");
        }

        var waitTime = timeout ?? TimeSpan.FromSeconds(15);
        var endTime = DateTime.UtcNow + waitTime;

        Console.WriteLine(
            $"Searching desktop to close window: '{windowName}'");

        while (DateTime.UtcNow < endTime)
        {
            var desktop = AutomationManager.Automation.GetDesktop();

            var windows = desktop.FindAllChildren(
                cf => cf.ByControlType(
                    FlaUI.Core.Definitions.ControlType.Window));

            foreach (var element in windows)
            {
                try
                {
                    if (!string.Equals(
                        element.Name?.Trim(),
                        windowName.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var window = element.AsWindow();

                    Console.WriteLine(
                        $"Closing window: '{window.Title}'");

                    window.Close();

                    Console.WriteLine(
                        $"Close command sent to: '{windowName}'");

                    // Do not change MainWindow unless it referred
                    // to the window that was just closed.
                    if (AutomationManager.MainWindow != null &&
                        string.Equals(
                            AutomationManager.MainWindow.Title?.Trim(),
                            windowName.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        AutomationManager.MainWindow = null;
                    }

                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Could not close window '{windowName}': " +
                        ex.Message);
                }
            }

            await Task.Delay(500);
        }

        throw new TimeoutException(
            $"Window '{windowName}' was not found on the desktop " +
            $"within {waitTime.TotalSeconds} seconds.");
    }

    public static void ClickSaveMeasurementOK()
    {
        var automation = AutomationManager.Automation
            ?? throw new InvalidOperationException(
                "UI Automation session has not started.");

        var desktop = automation.GetDesktop();

        var windows = desktop.FindAllChildren(
            cf => cf.ByControlType(
                FlaUI.Core.Definitions.ControlType.Window));

        foreach (var element in windows)
        {
            var window = element.AsWindow();

            var saveLabel = window.FindFirstDescendant(
                cf => cf.ByName("Save measurement"));

            if (saveLabel == null)
                continue;

            Console.WriteLine(
                $"Found Save measurement dialog: '{window.Title}'");

            var ok = window.FindFirstDescendant(
                cf => cf.ByName("OK"));

            if (ok == null)
            {
                throw new InvalidOperationException(
                    "OK control was not found in Save measurement dialog.");
            }

            Console.WriteLine(
                $"OK control: Type='{ok.ControlType}', " +
                $"Bounds='{ok.BoundingRectangle}'");

            // Click the visible OK label within the button.
            ok.Click();

            Console.WriteLine("Clicked OK on Save measurement dialog.");
            return;
        }

        throw new InvalidOperationException(
            "Save measurement dialog was not found.");
    }

}
