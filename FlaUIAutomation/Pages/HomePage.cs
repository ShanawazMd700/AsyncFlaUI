using FlaUI.Core.WindowsAPI;
using FlaUIAutomation.Drivers;
using FlaUIAutomation.Utilities;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using FlaUI.Core.Input;

namespace FlaUIAutomation.Pages;

public class HomePage : ControlHelpers
{
    private const string PatientDatabaseButtonAutomationId = "fittingpath_button_patient-database"; // AutomationId    fittingpath_button_patient-database
    private const string NewpatientButtonname = "patientlist_link_import"; // AutomationId    patientlist_link_import
    private const string Firstnamefield = "patientprofile_input-text_first-name"; // AutomationId    patientprofile_input-text_first-name
    private const string Lastnamefield = "patientprofile_input-text_last-name"; //AutomationId	patientprofile_input-text_last-name

    private const string audiogramTab = "patientdetailslayout_link_audiogram"; // AutomationId    patientdetailslayout_link_audiogram
    private const string audiogramFieldValue ="audiogramgrid_input-text_ac-125"; //AutomationId audiogramgrid_input-text_ac - 125
    private const string copytoright = "patientaudiogram_button_copy-right"; // AutomationId patientaudiogram_button_copy-right
    private const string newsessionbutton = "savepatientconfirmationdialog_button_fit"; // AutomationId    savepatientconfirmationdialog_button_fit
    private const string savebutton = "patientdetailslayout_button_save"; // AutomationId    patientdetailslayout_button_save
    private const string simulatesession = "fittingpath_link_simulate"; // AutomationId	fittingpath_link_simulate
    private const string simulatebutton = "instruments_button_simulate"; //AutomationId	instruments_button_simulate
    private const string pysicalproperties_continue = "StateMachineAutomationIds.ContinueAction"; //AutomationId	StateMachineAutomationIds.ContinueAction
    private const string viewbutton_patientlist = "patientlistitem_button_view"; // AutomationId	patientlistitem_button_view



    public async Task ClickPatientDatabaseAsync()
    {
        var patientDatabaseButton =
            await SessionManager.SwitchToWindowContainingAutomationIdAsync(
                PatientDatabaseButtonAutomationId,
                timeout: TimeSpan.FromSeconds(60));
        await ClickElementAsync(
            patientDatabaseButton);
    }



    public async Task AddNewpatientAsync(
        string firstName,
        string lastName)
    {
        var newPatientButton =
            await SessionManager.SwitchToWindowContainingAutomationIdAsync(NewpatientButtonname);
        await ClickElementAsync(
            newPatientButton);

        // Keep the rest of your existing patient-creation steps here.

        // 3. Enter patient details.
        await EnterTextByAutomationIdAsync(
            Firstnamefield, firstName);

        await Task.Delay(1000);

        await EnterTextByAutomationIdAsync(
            Lastnamefield, lastName);

        // 4. Enter audiogram values.
        await enterAudiogramValuesAsync();

        await Task.Delay(2000);

        // 5. Copy audiogram values and save.
        await ClickByAutomationIdAsync(
            copytoright);

        await ClickByAutomationIdAsync(
            savebutton);

        await Task.Delay(2000);

        // 6. Start the fitting session.
        await ClickByAutomationIdAsync(
            newsessionbutton);

        await Task.Delay(2000);

        // 7. Switch to the window containing Simulate.
        var simulateElement =
            await SessionManager.SwitchToWindowContainingAutomationIdAsync(simulatesession);

        await Task.Delay(2000);

        // 8. Click Simulate.
        await ClickElementAsync(
            simulateElement);
    }


    private async Task enterAudiogramValuesAsync()
    {
        await ClickByAutomationIdAsync(audiogramTab);

        // Focus the first AC cell (125 Hz).
        await ClickByAutomationIdAsync("audiogramgrid_input-text_ac-125");
        await Task.Delay(1000);
        await ClickByAutomationIdAsync("audiogramgrid_input-text_ac-125");

        string[] values =
        {
        "40", "40", "40", "40", "40", "40",
        "40", "40", "40", "40", "40"
    };

        foreach (string value in values)
        {
            Keyboard.Type(value);
            Keyboard.Press(VirtualKeyShort.TAB);
            await Task.Delay(1000);
        }
    }
    private void printTree() //string deviceName
    {
        var allElements = AutomationManager.MainWindow!
            .FindAllDescendants();
        foreach (var e in allElements)
        {
            try
            {
                var automationId =
                    e.Properties.AutomationId.ValueOrDefault;
                if (!string.IsNullOrEmpty(automationId))
                {
                    Console.WriteLine(
                        $"Name='{e.Name}' | AutomationId='{automationId}'");
                }
            }
            catch
            {
                // Ignore Chrome elements that don't support AutomationId
            }
        }
        //ClickByAutomationId(deviceName);
    }
    private async Task SelectDeviceAsync(string deviceName, string side)
    {
        string normalizedDevice = deviceName
       .Trim()
       .ToLowerInvariant();

        string normalizedSide = side
            .Trim()
            .ToLowerInvariant();

        if (normalizedSide == "both")
        {
            (await FindByNameAsync("Both")).Click();
            return;
        }

        if (normalizedSide != "right" && normalizedSide != "left")
        {
            throw new ArgumentException(
                "Side must be Right, Left, or Both.",
                nameof(side));
        }

        string automationId =
            $"catalogproduct_radio-button_{normalizedSide}-{normalizedDevice}";

        Console.WriteLine(
            $"Selecting device: {deviceName}, Side: {side}");

        await ClickByAutomationIdAsync(automationId);
        await ClickByAutomationIdAsync(simulatebutton);
    }
    
    public async Task SelectDeviceWithFullnameAsync(string deviceName, string side)
    {
        if (deviceName.Contains("sn") || deviceName.Contains("SN"))
        {
            (await FindByNameAsync("ReSound Sensia")).Click();
            await SelectDeviceAsync(deviceName, side);
        }
    }
    public async Task NavigateToFitScreeenAsync()
    {
        await Task.Delay(3000);
        await SessionManager.SwitchToWindowContainingAutomationIdAsync(pysicalproperties_continue);
        await ClickByAutomationIdAsync(pysicalproperties_continue);
        await Task.Delay(3000);
        await ClickByAutomationIdAsync(pysicalproperties_continue);
    }
    public async Task SelectExixtingPatientAsync(string firstName)
    {
        await SessionManager.SwitchToWindowContainingAutomationIdAsync(NewpatientButtonname);
        var element = await FindByNameAsync(firstName);
        MoveMouseToElement(element);
        await ClickByAutomationIdAsync(viewbutton_patientlist);
    }
}
