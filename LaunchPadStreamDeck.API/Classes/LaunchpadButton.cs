using LaunchPadStreamDeck.API.Enums;
using Midi;

namespace LaunchPadStreamDeck.API.Classes
{
    public class LaunchpadButton
    {
        private LaunchpadDevice mLaunchpadDevice;
        private ButtonBrightness mRedBrightness;
        private ButtonBrightness mGreenBrightness;
        private ButtonPressState mState;
        private ButtonType mType;
        public int mIndex;

        internal LaunchpadButton(LaunchpadDevice launchpadDevice, ButtonType type, int index)
        {
            this.mLaunchpadDevice = launchpadDevice;
            this.mType = type;
            this.mIndex = index;
        }

        public void TurnOnLight() => this.SetBrightness(ButtonBrightness.Full, ButtonBrightness.Full);

        public void TurnOffLight() => this.SetBrightness(ButtonBrightness.Off, ButtonBrightness.Off);

        public void SetBrightness(ButtonBrightness red, ButtonBrightness green)
        {
            if (this.mRedBrightness == red && this.mGreenBrightness == green)
                return;
            this.mRedBrightness = red;
            this.mGreenBrightness = green;
            int num = (int)((ButtonBrightness)((int)this.mGreenBrightness << 4) | this.mRedBrightness);
            if (!this.mLaunchpadDevice.DoubleBuffered)
                num |= 12;
            this.SetLED(num);
        }

        public void SetLED(int value)
        {
            if (this.mType == ButtonType.Toolbar)
                this.mLaunchpadDevice.OutputDevice.SendControlChange(Channel.Channel1, (Control)this.mIndex, value);
            else
                this.mLaunchpadDevice.OutputDevice.SendNoteOn(Channel.Channel1, (Pitch)this.mIndex, value);
        }

        public ButtonBrightness RedBrightness
        {
            get => this.mRedBrightness;
            internal set => this.mRedBrightness = value;
        }

        public ButtonBrightness GreenBrightness
        {
            get => this.mGreenBrightness;
            internal set => this.mGreenBrightness = value;
        }

        public ButtonPressState State
        {
            get => this.mState;
            internal set => this.mState = value;
        }
    }
}
