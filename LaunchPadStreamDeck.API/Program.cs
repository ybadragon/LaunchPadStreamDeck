using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using LaunchPadStreamDeck.API.Classes;
using LaunchPadStreamDeck.API.Enums;
using LaunchPadStreamDeck.API.Models;
using LaunchPadStreamDeck.API.Services;

namespace LaunchPadStreamDeck.API
{
    class Program
    {
        private static LaunchpadService launchpadService;
        private static EventService eventService;
        private static InputSimulator inputSimulator = new InputSimulator();
        static void Main(string[] args)
        {
            try
            {
                launchpadService = new LaunchpadService();
                launchpadService.SetButtonPressed(ButtonPressed);
                launchpadService.SetButtonDown(ButtonDown);
                launchpadService.SetButtonUp(ButtonUp);
                eventService = new EventService();

                SaveTestKeyboardEvent(new KeyboardEvent
                {
                    ButtonX = 0,
                    ButtonY = 0,
                    EventName = "TestKeyboardEvent",
                    ProfileName = "Profile1",
                    ProfileNumber = 1,
                    VirtualKeyCodes = new List<VirtualKeyCode> { (VirtualKeyCode)(1 + 48), VirtualKeyCode.VK_K }
                });

                SaveTestProcessEvent(new ProcessEvent
                {
                    ButtonX = 1,
                    ButtonY = 0,
                    EventName = "TestProcessEvent",
                    ProfileName = "Profile1",
                    ProfileNumber = 1,
                    ProcessCommand = "notepad",
                });

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

        private static void SaveTestKeyboardEvent(KeyboardEvent keyboardEvent)
        {
            eventService.SaveEvent(keyboardEvent, new PositionEventOption {ButtonX = keyboardEvent.ButtonX, ButtonY = keyboardEvent.ButtonY, ProfileName = keyboardEvent.ProfileName}, true);
        }

        private static void SaveTestProcessEvent(ProcessEvent processEvent)
        {
            eventService.SaveEvent(processEvent, new NamedEventOption {EventName = processEvent.EventName, ProfileName = processEvent.ProfileName}, true);
        }


        private static void ButtonPressed(object sender, ButtonPressEventArgs e)
        {
            e.Button.RunTimedEvent(2000, 250, () =>
            {
                e.Button.SetNextBrightness(BrightnessDirection.Ascending);
                Console.WriteLine("Event Triggered");
            }, () =>
            {
                e.Button.SetBrightness(ButtonBrightness.Off, ButtonBrightness.Off);
                var option = ArgsToPositionEventOption(e, "Profile1");
                var tempEvent = eventService.GetEvent(option);
                if (tempEvent != null)
                {
                    if (tempEvent.EventType == EventType.Keyboard)
                    {
                        var kbEvent = tempEvent as KeyboardEvent;
                        inputSimulator.Keyboard.ModifiedKeyStroke(kbEvent.VirtualKeyModifiers, kbEvent.VirtualKeyCodes);
                    }
                    else
                    {
                        var procEvent = tempEvent as ProcessEvent;
                        Process.Start(new ProcessStartInfo(procEvent.ProcessCommand));
                    }
                }
                Console.WriteLine("Event Completed");
            });
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

        public static PositionEventOption ArgsToPositionEventOption(ButtonPressEventArgs e, string profileName)
        {
            return new PositionEventOption
            {
                ButtonX = e.X,
                ButtonY = e.Y,
                ProfileName = profileName
            };
        }
    }
}
