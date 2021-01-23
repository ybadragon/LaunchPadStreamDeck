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
        private static InputSimulator inputSimulator;
        private static LaunchpadDevice device;
        static void Main(string[] args)
        {
            try
            {
                device = new LaunchpadDevice(0, ButtonPressed, ToolBarButtonPressed, SideBarButtonPressed);
                inputSimulator = new InputSimulator();
                Console.WriteLine("Device Name: " + (device?.DeviceName ?? "Device not found"));
                while (true)
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to close");
                Console.ReadKey();
            }
        }

        private static void SideBarButtonPressed(object arg1, ButtonPressEventArgs arg2)
        {
            throw new NotImplementedException();
        }

        private static void ButtonPressed(object arg1, ButtonPressEventArgs arg2)
        {
            throw new NotImplementedException();
        }

        private static void ToolBarButtonPressed(object arg1, ButtonPressEventArgs arg2)
        {
            throw new NotImplementedException();
        }
    }
}
