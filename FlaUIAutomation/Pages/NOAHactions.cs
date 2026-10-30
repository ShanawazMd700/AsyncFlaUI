using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUIAutomation.Drivers;
using FlaUIAutomation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaUIAutomation.Pages
{
    public class NOAHactions : ControlHelpers
    {
        private const string OKButton = "cmdOK";
        private const string passwordfield = "Password";

        private const string fileoptionmenu = "_File";
        private const string lastnamefield = "LastName";
        private const string firstnamefield = "FirstName";


        public Task EnterPasswordAsync(string password, CancellationToken cancellationToken = default)
        {
            return EnterTextByAutomationIdAsync(passwordfield, password, cancellationToken);
        }

        public async Task ClickOKButtonAsync(CancellationToken cancellationToken = default)
        {
            await ClickByAutomationIdAsync(OKButton, cancellationToken);
            await Task.Delay(2000, cancellationToken);
        }



        public async Task AddPatientAsync(string fname, string lname, CancellationToken cancellationToken = default)
        {
            await SessionManager.ReattachMainWindowAsync(cancellationToken: cancellationToken);
            Console.WriteLine("========== AFTER REATTACH ==========");
            await ClickByAutomationIdAsync("cmdAddPatient", cancellationToken);
            await SessionManager.ReattachMainWindowAsync(cancellationToken: cancellationToken);
            await EnterTextByAutomationIdAsync(lastnamefield, lname, cancellationToken);
            await EnterTextByAutomationIdAsync(firstnamefield, fname, cancellationToken);
            (await FindByNameAsync("Male", cancellationToken: cancellationToken)).Click();
            (await FindByNameAsync("OK", cancellationToken: cancellationToken)).Click();
            await SessionManager.ReattachMainWindowAsync(cancellationToken: cancellationToken);
            await Task.Delay(4000, cancellationToken);
            // Open the module
            Console.WriteLine("===== STEP 1: BEFORE FILE MENU =====");
            (await FindByNameAsync("_File", cancellationToken: cancellationToken)).Click();
            await Task.Delay(1000, cancellationToken);
            Console.WriteLine("===== STEP 2: AFTER FILE MENU =====");
            Console.WriteLine("===== STEP 3: CLICK OPEN =====");
            (await FindByNameAsync("Open", cancellationToken: cancellationToken)).Click();
            await Task.Delay(1000, cancellationToken);
            (await FindByNameAsync("Noah 4 Audiogram Module", cancellationToken: cancellationToken)).Click();
            await Task.Delay(2000, cancellationToken);
            await SessionManager.SwitchToWindowAsync("Audiogram Module", cancellationToken: cancellationToken);
            Console.WriteLine("===== STEP 4: AFTER OPEN =====");
            Console.WriteLine("===============Inspect Audiogram Grid==================");
            await Task.Delay(1500, cancellationToken);
            //SessionManager.InspectAudiogramGrid();

            await enterAudiogramValuesAsync(cancellationToken);
            await SessionManager.CloseWindowAsync("Audiogram Module", cancellationToken: cancellationToken);
            await Task.Delay(3000, cancellationToken);
            SessionManager.ClickSaveMeasurementOK();

            await Task.Delay(6000, cancellationToken);
            //SessionManager.CloseWindow("Audiogram Module");
        }
        private const string audiogramcell = "Item: Himsa.Noah.CommonControls.TestValuesViewModel, Column Display Index: 0"; // Name	Item: Himsa.Noah.CommonControls.TestValuesViewModel, Column Display Index: 0
        private async Task enterAudiogramValuesAsync(CancellationToken cancellationToken)
        {
            var window = AutomationManager.MainWindow
                ?? throw new InvalidOperationException(
                    "Audiogram Module is not attached.");
            AutomationElement? cell = null;
            // Retry because the module controls may take time to load.
            for (int attempt = 1; attempt <= 10; attempt++)
            {
                cell = window.FindFirstDescendant(
                    cf => cf.ByName(audiogramcell));
                if (cell != null)
                    break;
                await Task.Delay(500, cancellationToken);
            }
            if (cell == null)
            {
                Console.WriteLine(
                    "Audiogram cell not found using exact name: " +
                    audiogramcell);

                var candidates = window.FindAllDescendants()
                    .Where(e =>
                    {
                        try
                        {
                            return e.Name?.Contains(
                                "TestValuesViewModel",
                                StringComparison.OrdinalIgnoreCase) == true;
                        }
                        catch
                        {
                            return false;
                        }
                    });

                foreach (var candidate in candidates)
                {
                    Console.WriteLine(
                        $"Candidate: Name='{candidate.Name}', " +
                        $"Type='{candidate.ControlType}'");
                }

                throw new InvalidOperationException(
                    "Audiogram cell was not found. Check the candidate output.");
            }
            cell.Click();
            await Task.Delay(200, cancellationToken);
            Keyboard.Type("50");
            await Task.Delay(200, cancellationToken);
            for (int i = 0; i < 13; i++)
            {
                Keyboard.Press(VirtualKeyShort.TAB);
                await Task.Delay(100, cancellationToken);
                Keyboard.Press(VirtualKeyShort.TAB);
                await Task.Delay(100, cancellationToken);
                Keyboard.Type("40");
                await Task.Delay(200, cancellationToken);
            }
        }

        private static void PrintElementTree(
    AutomationElement element,
    int level = 0)
        {
            string indent = new string(' ', level * 2);

            string name = "";
            string automationId = "";
            string controlType = "";

            try
            {
                name = element.Name;
            }
            catch
            {
            }

            try
            {
                automationId =
                    element.Properties.AutomationId.ValueOrDefault ?? "";
            }
            catch
            {
            }

            try
            {
                controlType =
                    element.ControlType.ToString();
            }
            catch
            {
            }

            Console.WriteLine(
                $"{indent}" +
                $"Name='{name}' | " +
                $"AutomationId='{automationId}' | " +
                $"ControlType='{controlType}'");

            foreach (var child in element.FindAllChildren())
            {
                PrintElementTree(child, level + 1);
            }
        }
        public async Task LaunchFswAsync(string fsw, CancellationToken cancellationToken = default)
        {
            await SessionManager.SwitchToWindowAsync("Noah 4", cancellationToken: cancellationToken);
            (await FindByNameAsync("_File", cancellationToken: cancellationToken)).Click();
            await Task.Delay(1000, cancellationToken);
            (await FindByNameAsync("Open", cancellationToken: cancellationToken)).Click();
            await Task.Delay(1000, cancellationToken);
            (await FindByNameContainsAsync(fsw, cancellationToken: cancellationToken)).Click();
        }

    }
}
