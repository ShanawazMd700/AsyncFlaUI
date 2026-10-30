
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUIAutomation.Drivers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlaUIAutomation.Utilities;

public static class WaitHelper
{
    public static async Task<AutomationElement> WaitForElementByAutomationIdAsync(
        string automationId,
        int timeout = 60,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(automationId);

        var automation = AutomationManager.Automation
            ?? throw new InvalidOperationException(
                "UI Automation is not initialized. Call SessionManager.StartSession() first.");

        var endTime = DateTime.UtcNow.AddSeconds(timeout);

        while (DateTime.UtcNow < endTime)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var desktop = automation.GetDesktop();
            var windows = desktop.FindAllChildren(
                cf => cf.ByControlType(ControlType.Window));

            foreach (var window in windows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var element = window.FindFirstDescendant(
                        cf => cf.ByAutomationId(automationId));

                    if (element != null)
                    {
                        Console.WriteLine(
                            $"Found AutomationId='{automationId}' " +
                            $"in window '{window.Name}'.");

                        return element;
                    }
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    Console.WriteLine(
                        $"Could not inspect a window for '{automationId}': {ex.Message}");
                }
            }

            await Task.Delay(500, cancellationToken);
        }

        throw new TimeoutException(
            $"AutomationId '{automationId}' was not found in any top-level window " +
            $"within {timeout} seconds.");
    }

    public static async Task<AutomationElement> WaitForElementByNameAsync(
        string name,
        int timeout = 60,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var automation = AutomationManager.Automation
            ?? throw new InvalidOperationException(
                "UI Automation is not initialized. Call SessionManager.StartSession() first.");

        var endTime = DateTime.UtcNow.AddSeconds(timeout);

        while (DateTime.UtcNow < endTime)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var desktop = automation.GetDesktop();
            var windows = desktop.FindAllChildren(
                cf => cf.ByControlType(ControlType.Window));

            foreach (var window in windows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var element = window.FindFirstDescendant(
                        cf => cf.ByName(name));

                    if (element != null)
                    {
                        Console.WriteLine(
                            $"Found Name='{name}' in window '{window.Name}'.");

                        return element;
                    }
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    Console.WriteLine(
                        $"Could not inspect a window for '{name}': {ex.Message}");
                }
            }

            await Task.Delay(500, cancellationToken);
        }

        throw new TimeoutException(
            $"Element with name '{name}' was not found in any top-level window " +
            $"within {timeout} seconds.");
    }
}
