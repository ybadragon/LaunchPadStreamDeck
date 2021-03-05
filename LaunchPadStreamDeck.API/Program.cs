﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using WindowsInput;
using WindowsInput.Native;
using LaunchPadStreamDeck.API.Classes;
using LaunchPadStreamDeck.API.Enums;

namespace LaunchPadStreamDeck.API
{
    class Program
    {
        private static LaunchpadService launchpadService;
        static void Main(string[] args)
        {
            try
            {
                launchpadService = new LaunchpadService();
                launchpadService.SetButtonPressed(ButtonPressed);
                launchpadService.SetButtonDown(ButtonDown);
                launchpadService.SetButtonUp(ButtonUp);
                Console.WriteLine("Device Name: " + (launchpadService.Device?.DeviceName ?? "Device not found"));
                launchpadService.StartListening();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to close");
                Console.ReadKey();
            }
        }

        private static void ButtonPressed(object sender, ButtonPressEventArgs e)
        {
            Console.WriteLine($"ButtonPressed: {e.ButtonDescription}");
        }

        private static void ButtonUp(object sender, ButtonPressEventArgs e)
        {
            Console.WriteLine($"ButtonUp: {e.ButtonDescription}");
        }

        private static void ButtonDown(object sender, ButtonPressEventArgs e)
        {
            Console.WriteLine($"ButtonDown: {e.ButtonDescription}");
        }
    }
}
