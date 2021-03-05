using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using LaunchPadStreamDeck.API.Enums;

namespace LaunchPadStreamDeck.API.Classes
{
    public class LaunchpadService
    {
        public LaunchpadDevice Device;
        private bool isListening;

        public LaunchpadService()
        {
            Device = new LaunchpadDevice(0);
        }

        public void SetButtonPressed(Action<object, ButtonPressEventArgs> buttonPressed)
        {
            Device.ButtonPressed += (o, ea) => buttonPressed(o, ea);
        }

        public void SetButtonUp(Action<object, ButtonPressEventArgs> buttonUp)
        {
            Device.ButtonUp += (o, ea) => buttonUp(o, ea);
        }

        public void SetButtonDown(Action<object, ButtonPressEventArgs> buttonDown)
        {
            Device.ButtonDown += (o, ea) => buttonDown(o, ea);
        }

        public void StartListening()
        {
            isListening = true;
            while (isListening)
            { 
            }
        }

        public void StopListening()
        {
            isListening = false;
        }

        public void ToggleLight(LaunchpadButton button)
        {
            ButtonBrightness red = button.RedBrightness != ButtonBrightness.Off
                ? ButtonBrightness.Off
                : ButtonBrightness.Full;

            ButtonBrightness green = button.GreenBrightness != ButtonBrightness.Off
                ? ButtonBrightness.Off
                : ButtonBrightness.Full;

            button.SetBrightness(red, green);
        }

        public void TurnOffLight(LaunchpadButton button)
        {
            button.SetBrightness(ButtonBrightness.Off, ButtonBrightness.Off);
        }

        public void SetButtonBrightness(LaunchpadButton button, ButtonBrightness redBrightness,
            ButtonBrightness greenBrightness)
        {
            button.SetBrightness(redBrightness, greenBrightness);
        }

        public LaunchpadButton GetSideBarButton(int index)
        {
            return Device.GetButton((SideButton)index);
        }

        public LaunchpadButton GetSideBarButton(SideButton sideBarButton)
        {
            return Device.GetButton(sideBarButton);
        }

        public LaunchpadButton GetToolBarButton(int index)
        {
            return Device.GetButton((ToolbarButton) index);
        }

        public LaunchpadButton GetToolBarButton(ToolbarButton toolbarButton)
        {
            return Device.GetButton(toolbarButton);
        }
    }
}
