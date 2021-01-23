using System;
using System.Collections.Generic;
using System.Diagnostics;
using WindowsInput;
using WindowsInput.Native;
using LaunchPadStreamDeck.API.Classes;

namespace LaunchPadStreamDeck.API
{
    class Program
    {
        private static LaunchpadService launchpadService;
        static void Main(string[] args)
        {
            try
            {
                launchpadService = new LaunchpadService(ButtonPressed, ToolBarButtonPressed, SideBarButtonPressed);
                Console.WriteLine("Device Name: " + (launchpadService.Device?.DeviceName ?? "Device not found"));
                launchpadService.WaitForInput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to close");
                Console.ReadKey();
            }
        }

        private static void SideBarButtonPressed(object sender, ButtonPressEventArgs e)
        {
            launchpadService.ToggleLight(e.Button);
        }

        private static void ButtonPressed(object sender, ButtonPressEventArgs e)
        {
            launchpadService.ToggleLight(e.Button);
        }

        private static void ToolBarButtonPressed(object sender, ButtonPressEventArgs e)
        {
            launchpadService.ToggleLight(e.Button);
        }
    }
}
