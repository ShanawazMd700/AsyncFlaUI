using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUIAutomation.Drivers;
using FlaUI.Core.Conditions;
using System;
using FlaUI.Core.Definitions;

namespace FlaUIAutomation.Utilities;

public class ControlHelpers
{
    protected Task<AutomationElement> FindByAutomationIdAsync(string automationId, int timeout = 90) =>
        WaitHelper.WaitForElementByAutomationIdAsync(automationId, timeout);
    protected Task<AutomationElement> FindByNameAsync(string name, int timeout = 90) =>
        WaitHelper.WaitForElementByNameAsync(name, timeout);

    protected async Task ClickByAutomationIdAsync(
        string automationId)
    {
        var element = await FindByAutomationIdAsync(
            automationId);

        if (element == null)
        {
            throw new InvalidOperationException(
                $"WaitHelper returned null for AutomationId '{automationId}'.");
        }

        Console.WriteLine(
            $"Clicking AutomationId='{automationId}', " +
            $"Name='{element.Name}', Type='{element.ControlType}'");

        element.Click();
    }

    protected async Task EnterTextByAutomationIdAsync(string automationId, string text)
    {
        var element = await FindByAutomationIdAsync(automationId);
        element.Focus();
        element.AsTextBox().Enter(text);
    }
    protected async Task EnterTextByNameAsync(string name, string text)
    {
        var element = await FindByNameAsync(name);
        element.Focus();
        element.AsTextBox().Enter(text);
    }
    protected async Task<string> GetTextByAutomationIdAsync(string automationId)
    {
        var element = await FindByAutomationIdAsync(automationId);
        return element.AsLabel()?.Text ?? element.Name;
    }
    protected async Task<bool> IsDisplayedByAutomationIdAsync(string automationId) =>
        !(await FindByAutomationIdAsync(automationId)).Properties.IsOffscreen.ValueOrDefault;

    public static async Task ClickElementAsync(
    AutomationElement element,
    bool useCoordinateFallback = true)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        Console.WriteLine(
            $"Clicking element: Name='{element.Name}', " +
            $"Type='{element.ControlType}', " +
            $"AutomationId='{element.AutomationId}'");

        // Attempt 1: Use the element's normal click implementation.
        try
        {
            element.Click();
            Console.WriteLine("Clicked using UI Automation.");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Direct click failed: {ex.Message}");
        }

        // Attempt 2: Click the parent if it is a button or another
        // likely clickable container.
        try
        {
            var parent = element.Parent;

            if (parent != null &&
                (parent.ControlType ==
                     FlaUI.Core.Definitions.ControlType.Button ||
                 parent.ControlType ==
                     FlaUI.Core.Definitions.ControlType.Custom))
            {
                parent.Click();
                Console.WriteLine("Clicked using parent element.");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Parent click failed: {ex.Message}");
        }

        // Attempt 3: Click the center of the element's screen bounds.
        if (useCoordinateFallback)
        {
            var bounds = element.BoundingRectangle;

            if (bounds.IsEmpty ||
                bounds.Width <= 0 ||
                bounds.Height <= 0)
            {
                throw new InvalidOperationException(
                    "Cannot click element: it has no usable screen bounds.");
            }

            var x = (int)(bounds.Left + bounds.Width / 2);
            var y = (int)(bounds.Top + bounds.Height / 2);

            Mouse.Click(new System.Drawing.Point(x, y));
            await Task.Delay(200);

            Console.WriteLine(
                $"Clicked using coordinates ({x}, {y}).");

            return;
        }

        throw new InvalidOperationException(
            $"Could not click element '{element.Name}'.");
    }

    public static async Task ClickByNameAsync(
        AutomationElement root,
        string name,
    bool useCoordinateFallback = true)
    {
        var element = root.FindFirstDescendant(
            cf => cf.ByName(name));

        if (element == null)
        {
            throw new InvalidOperationException(
                $"Element with name '{name}' was not found.");
        }

        await ClickElementAsync(element, useCoordinateFallback);
    }
    public async Task<AutomationElement> FindByNameContainsAsync(string name, int timeout = 10)
    {
        var window = AutomationManager.MainWindow
            ?? throw new InvalidOperationException("Main window is not attached.");

        var endTime = DateTime.UtcNow.AddSeconds(timeout);
        while (DateTime.UtcNow < endTime)
        {
            var element = window.FindFirstDescendant(cf => cf.ByName(name, PropertyConditionFlags.MatchSubstring));
            if (element != null) return element;
            await Task.Delay(250);
        }
        throw new TimeoutException($"Element containing name '{name}' was not found.");
    }
    public static void MoveMouseToElement(AutomationElement element)
{
    if (element == null)
    {
        throw new ArgumentNullException(nameof(element));
    }

    var bounds = element.BoundingRectangle;

    if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
    {
        throw new InvalidOperationException(
            $"Cannot move mouse: element '{element.Name}' has no usable screen bounds.");
    }

    int x = (int)(bounds.Left + bounds.Width / 2);
    int y = (int)(bounds.Top + bounds.Height / 2);

    Mouse.MoveTo(new System.Drawing.Point(x, y));

    Console.WriteLine(
        $"Moved mouse to element: Name='{element.Name}', X={x}, Y={y}");
}
}
