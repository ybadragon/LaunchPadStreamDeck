using Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using LaunchPadStreamDeck.API.Enums;

namespace LaunchPadStreamDeck.API.Classes
{
    public class LaunchpadDevice
    {
        private InputDevice mInputDevice;
        private OutputDevice mOutputDevice;
        private bool mDoubleBuffered;
        private bool mDoubleBufferedState;
        private readonly LaunchpadButton[] mToolbar = new LaunchpadButton[8];
        private readonly LaunchpadButton[] mSide = new LaunchpadButton[8];
        private readonly LaunchpadButton[,] mGrid = new LaunchpadButton[8, 8];
        public readonly string DeviceName;

        public event EventHandler<ButtonPressEventArgs> ButtonPressed;
        public event EventHandler<ButtonPressEventArgs> ButtonUp;
        public event EventHandler<ButtonPressEventArgs> ButtonDown;


        public LaunchpadDevice(
          int index)
        {
            InitialiseButtons();
            int i = 0;
            mInputDevice = InputDevice.InstalledDevices.Where(x => x.Name.Contains("Launchpad")).FirstOrDefault(x => i++ == index);
            i = 0;
            mOutputDevice = OutputDevice.InstalledDevices.Where(x => x.Name.Contains("Launchpad")).FirstOrDefault(x => i++ == index);
            DeviceName = mInputDevice != null ? mInputDevice.Name : throw new LaunchpadException("Unable to find input device.");
            if (mOutputDevice == null)
                throw new LaunchpadException("Unable to find output device.");
            mInputDevice.Open();
            mOutputDevice.Open();
            mInputDevice.StartReceiving(new Clock(120f));
            mInputDevice.NoteOn += mInputDevice_NoteOn;
            mInputDevice.ControlChange += mInputDevice_ControlChange;
            Reset();
        }

        private void InitialiseButtons()
        {
            for (int index = 0; index < 8; ++index)
            {
                mToolbar[index] = new LaunchpadButton(this, ButtonType.Toolbar, 104 + index);
                mSide[index] = new LaunchpadButton(this, ButtonType.Side, index * 16 + 8);
            }
            for (int index1 = 0; index1 < 8; ++index1)
            {
                for (int index2 = 0; index2 < 8; ++index2)
                    mGrid[index2, index1] = new LaunchpadButton(this, ButtonType.Grid, index1 * 16 + index2);
            }
        }

        private void StartDoubleBuffering()
        {
            mDoubleBuffered = true;
            mDoubleBufferedState = false;
            mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 49);
        }

        public void Refresh()
        {
            if (!mDoubleBufferedState)
                mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 52);
            else
                mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 49);
            mDoubleBufferedState = !mDoubleBufferedState;
        }

        private void EndDoubleBuffering()
        {
            mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 48);
            mDoubleBuffered = false;
        }

        public void Reset()
        {
            mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 0);
            Buttons.ToList<LaunchpadButton>().ForEach((Action<LaunchpadButton>)(x => x.RedBrightness = x.GreenBrightness = ButtonBrightness.Off));
        }

        private void mInputDevice_NoteOn(NoteOnMessage msg)
        {
            LaunchpadButton button = GetButton(msg.Pitch);
            if (button == null)
                return;
            button.State = (ButtonPressState)msg.Velocity;
            if (ButtonPressed == null)
                return;


            var pressEventArgs = (int)msg.Pitch % 16 == 8 ? new ButtonPressEventArgs((SideButton)((int)msg.Pitch / 16), button) : new ButtonPressEventArgs((int) msg.Pitch % 16, (int) msg.Pitch / 16, button);

            if (button.State == ButtonPressState.Up)
            {
                ButtonPressed?.Invoke(this, pressEventArgs);
                ButtonUp?.Invoke(this, pressEventArgs);
            }
            else
            {
                ButtonDown?.Invoke(this, pressEventArgs);
            }
        }

        private void mInputDevice_ControlChange(ControlChangeMessage msg)
        {
            ToolbarButton toolbarButton = (ToolbarButton)(msg.Control - 104);
            LaunchpadButton button = GetButton(toolbarButton);
            if (button == null)
                return;
            button.State = (ButtonPressState)msg.Value;

            var pressEventArgs = new ButtonPressEventArgs(toolbarButton, button);

            if (button.State == ButtonPressState.Up)
            {
                ButtonPressed?.Invoke(this, pressEventArgs);
                ButtonUp?.Invoke(this, pressEventArgs);
            }
            else
            {
                ButtonDown?.Invoke(this, pressEventArgs);
            }
        }

        public LaunchpadButton GetButton(ToolbarButton toolbarButton) => mToolbar[(int)toolbarButton];

        public LaunchpadButton GetButton(SideButton sideButton) => mSide[(int)sideButton];

        private LaunchpadButton GetButton(Pitch pitch)
        {
            int index1 = (int)pitch % 16;
            int index2 = (int)pitch / 16;
            if (index1 < 8 && index2 < 8)
                return mGrid[index1, index2];
            return index1 == 8 && index2 < 8 ? mSide[index2] : (LaunchpadButton)null;
        }

        public bool DoubleBuffered
        {
            get => mDoubleBuffered;
            set
            {
                if (mDoubleBuffered)
                    EndDoubleBuffering();
                else
                    StartDoubleBuffering();
            }
        }

        public LaunchpadButton this[int x, int y] => mGrid[x, y];

        public IEnumerable<LaunchpadButton> Buttons
        {
            get
            {
                for (int y = 0; y < 8; ++y)
                {
                    for (int x = 0; x < 8; ++x)
                        yield return mGrid[x, y];
                }
            }
        }

        internal OutputDevice OutputDevice => mOutputDevice;
    }
}
