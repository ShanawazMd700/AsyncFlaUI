using System;
using System.Collections.Generic;
using System.Text;
using FlaUI.Core.WindowsAPI;
using FlaUIAutomation.Drivers;
using FlaUIAutomation.Utilities;
using FlaUI.Core.Input;

namespace FlaUIAutomation.Pages
{
    public class FittingScreen : ControlHelpers
    {
        private const string savebutton = "PatientAutomationIds.ProfileAutomationIds.SaveAction"; // AutomationId	PatientAutomationIds.ProfileAutomationIds.SaveAction
        private const string continuebutton = "SaveAutomationIds.PerformSaveAutomationIds.ContinueAction"; //AutomationId	SaveAutomationIds.PerformSaveAutomationIds.ContinueAction
        private const string exitbutton = "SaveAutomationIds.PerformSaveAutomationIds.ExitAction"; //AutomationId	SaveAutomationIds.PerformSaveAutomationIds.ExitAction



        public async Task SaveSessionAsync()
        {
            await ClickByAutomationIdAsync(savebutton);
            //SessionManager.SwitchToWindowContainingAutomationId(exitbutton);
            await Task.Delay(5000);
            await ClickByAutomationIdAsync(exitbutton);
            await Task.Delay(2000);
        }

    }
}
