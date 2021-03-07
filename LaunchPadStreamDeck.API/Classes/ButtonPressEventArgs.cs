using System;
using LaunchPadStreamDeck.API.Enums;
using LaunchPadStreamDeck.API.Models;

namespace LaunchPadStreamDeck.API.Classes
{
    public class ButtonPressEventArgs : EventArgs
    {
        private ButtonType mType;
        private ToolbarButton mToolbarButton;
        private SideButton mSidebarButton;
        private int mX;
        private int mY;
        private int mIndex;
        private LaunchpadButton mButton;

        public ButtonPressEventArgs(ToolbarButton toolbarButton, LaunchpadButton button)
        {
            this.mType = ButtonType.Toolbar;
            this.mToolbarButton = toolbarButton;
            this.mButton = button;
        }

        public ButtonPressEventArgs(SideButton sideButton, LaunchpadButton button)
        {
            this.mType = ButtonType.Side;
            this.mSidebarButton = sideButton;
            this.mButton = button;
        }

        public ButtonPressEventArgs(int x, int y, LaunchpadButton button, int index = 0)
        {
            this.mType = ButtonType.Grid;
            this.mX = x;
            this.mY = y;
            this.mButton = button;
            this.mIndex = index;
        }

        public ButtonType Type => this.mType;

        public ToolbarButton ToolbarButton => this.mToolbarButton;

        public SideButton SidebarButton => this.mSidebarButton;

        public int X => this.mX;

        public int Y => this.mY;

        public int Index => this.mIndex;

        public LaunchpadButton Button => this.mButton;

        public string ButtonDescription => $"ButtonType: {Type}, ButtonIndex: {Index}, ButtonX: {X}, ButtonY {Y}";
    }
}