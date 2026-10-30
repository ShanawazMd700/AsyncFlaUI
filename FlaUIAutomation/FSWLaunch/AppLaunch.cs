using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaUI.Core;
using FlaUIAutomation.Drivers;

namespace FlaUIAutomation.FSWLaunch
{
    public class AppLaunch
    {
        public async Task LaunchAppsAsync(string app)
        {
            switch (app)
            {
                case "smartfit":
                case "Smart Fit":
                case "SmartFit":
                    var application = Application.Launch(@"C:\Program Files (x86)\ReSound\SmartFit\SmartFit.exe");
                    Console.WriteLine($"ProcessId = {application.ProcessId}");
                    await SessionManager.AttachApplicationAsync(application);
                    Console.WriteLine($"Main Window Title: {AutomationManager.MainWindow?.Title}");
                    await Task.Delay(10000);
                    break;
                case "solusmax":
                case "Solus Max":
                    throw new NotImplementedException("Solus Max launch path is not configured.");

                case "NOAH":
                case "Noah":
                    var application1 = Application.Launch(@"C:\Program Files (x86)\HIMSA\Noah 4\Noah4.exe");
                    Console.WriteLine($"ProcessId = {application1.ProcessId}");
                    await SessionManager.AttachApplicationAsync(application1);
                    Console.WriteLine($"Main Window Title: {AutomationManager.MainWindow?.Title}");
                    await Task.Delay(10000);
                    break;

                case "FDTS":
                case "fdts":
                case "Camelot":
                    var application2 = Application.Launch(@"C:\Program Files (x86)\GN Hearing\Camelot\WorkflowRuntime\Camelot.WorkflowRuntime.exe");
                    Console.WriteLine($"ProcessId = {application2.ProcessId}");
                    await SessionManager.AttachApplicationAsync(application2);
                    Console.WriteLine($"Main Window Title: {AutomationManager.MainWindow?.Title}");
                    await Task.Delay(20000);
                    break;
                default:
                    throw new ArgumentException($"Unsupported FSW: {app}", nameof(app));
            }
        }
    }
}
