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

        public event EventHandler<ButtonPressEventArgs> ToolBarPressed;

        public event EventHandler<ButtonPressEventArgs> SideBarPressed;

        public LaunchpadDevice()
          : this(0, (Action<object, ButtonPressEventArgs>)null, (Action<object, ButtonPressEventArgs>)null, (Action<object, ButtonPressEventArgs>)null)
        {
        }

        public LaunchpadDevice(
          int index,
          Action<object, ButtonPressEventArgs> buttonPressed,
          Action<object, ButtonPressEventArgs> toolBarButtonPressed,
          Action<object, ButtonPressEventArgs> sideBarButtonPressed)
        {
            this.InitialiseButtons();
            int i = 0;
            this.mInputDevice = InputDevice.InstalledDevices.Where<InputDevice>((Func<InputDevice, bool>)(x => x.Name.Contains("Launchpad"))).FirstOrDefault<InputDevice>((Func<InputDevice, bool>)(x => i++ == index));
            i = 0;
            this.mOutputDevice = OutputDevice.InstalledDevices.Where<OutputDevice>((Func<OutputDevice, bool>)(x => x.Name.Contains("Launchpad"))).FirstOrDefault<OutputDevice>((Func<OutputDevice, bool>)(x => i++ == index));
            this.DeviceName = this.mInputDevice != null ? this.mInputDevice.Name : throw new LaunchpadException("Unable to find input device.");
            if (this.mOutputDevice == null)
                throw new LaunchpadException("Unable to find output device.");
            this.mInputDevice.Open();
            this.mOutputDevice.Open();
            this.mInputDevice.StartReceiving(new Clock(120f));
            this.mInputDevice.NoteOn += new InputDevice.NoteOnHandler(this.mInputDevice_NoteOn);
            this.mInputDevice.ControlChange += new InputDevice.ControlChangeHandler(this.mInputDevice_ControlChange);
            this.ButtonPressed += (EventHandler<ButtonPressEventArgs>)((o, ea) => buttonPressed(o, ea));
            this.ToolBarPressed += (EventHandler<ButtonPressEventArgs>)((o, ea) => toolBarButtonPressed(o, ea));
            this.SideBarPressed += (EventHandler<ButtonPressEventArgs>)((o, ea) => sideBarButtonPressed(o, ea));
            this.Reset();
        }

        private void InitialiseButtons()
        {
            for (int index = 0; index < 8; ++index)
            {
                this.mToolbar[index] = new LaunchpadButton(this, ButtonType.Toolbar, 104 + index);
                this.mSide[index] = new LaunchpadButton(this, ButtonType.Side, index * 16 + 8);
            }
            for (int index1 = 0; index1 < 8; ++index1)
            {
                for (int index2 = 0; index2 < 8; ++index2)
                    this.mGrid[index2, index1] = new LaunchpadButton(this, ButtonType.Grid, index1 * 16 + index2);
            }
        }

        private void StartDoubleBuffering()
        {
            this.mDoubleBuffered = true;
            this.mDoubleBufferedState = false;
            this.mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 49);
        }

        public void Refresh()
        {
            if (!this.mDoubleBufferedState)
                this.mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 52);
            else
                this.mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 49);
            this.mDoubleBufferedState = !this.mDoubleBufferedState;
        }

        private void EndDoubleBuffering()
        {
            this.mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 48);
            this.mDoubleBuffered = false;
        }

        public void Reset()
        {
            this.mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 0);
            this.Buttons.ToList<LaunchpadButton>().ForEach((Action<LaunchpadButton>)(x => x.RedBrightness = x.GreenBrightness = ButtonBrightness.Off));
        }

        private void mInputDevice_NoteOn(NoteOnMessage msg)
        {
            LaunchpadButton button = this.GetButton(msg.Pitch);
            if (button == null)
                return;
            button.State = (ButtonPressState)msg.Velocity;
            if (this.ButtonPressed == null || button.State != ButtonPressState.Down)
                return;
            if ((int)msg.Pitch % 16 == 8)
            {
                EventHandler<ButtonPressEventArgs> sideBarPressed = this.SideBarPressed;
                if (sideBarPressed != null)
                    sideBarPressed((object)this, new ButtonPressEventArgs((SideButton)((int)msg.Pitch / 16), button));
            }
            else
            {
                EventHandler<ButtonPressEventArgs> buttonPressed = this.ButtonPressed;
                if (buttonPressed != null)
                    buttonPressed((object)this, new ButtonPressEventArgs((int)msg.Pitch % 16, (int)msg.Pitch / 16, button));
            }
        }

        private void mInputDevice_ControlChange(ControlChangeMessage msg)
        {
            ToolbarButton toolbarButton = (ToolbarButton)(msg.Control - 104);
            LaunchpadButton button = this.GetButton(toolbarButton);
            if (button == null)
                return;
            button.State = (ButtonPressState)msg.Value;
            if (this.ToolBarPressed == null || button.State != ButtonPressState.Down)
                return;
            this.ToolBarPressed((object)this, new ButtonPressEventArgs(toolbarButton, button));
        }

        public LaunchpadButton GetButton(ToolbarButton toolbarButton) => this.mToolbar[(int)toolbarButton];

        public LaunchpadButton GetButton(SideButton sideButton) => this.mSide[(int)sideButton];

        private LaunchpadButton GetButton(Pitch pitch)
        {
            int index1 = (int)pitch % 16;
            int index2 = (int)pitch / 16;
            if (index1 < 8 && index2 < 8)
                return this.mGrid[index1, index2];
            return index1 == 8 && index2 < 8 ? this.mSide[index2] : (LaunchpadButton)null;
        }

        public bool DoubleBuffered
        {
            get => this.mDoubleBuffered;
            set
            {
                if (this.mDoubleBuffered)
                    this.EndDoubleBuffering();
                else
                    this.StartDoubleBuffering();
            }
        }

        public LaunchpadButton this[int x, int y] => this.mGrid[x, y];

        public IEnumerable<LaunchpadButton> Buttons
        {
            get
            {
                for (int y = 0; y < 8; ++y)
                {
                    for (int x = 0; x < 8; ++x)
                        yield return this.mGrid[x, y];
                }
            }
        }

        internal OutputDevice OutputDevice => this.mOutputDevice;
    }
}
