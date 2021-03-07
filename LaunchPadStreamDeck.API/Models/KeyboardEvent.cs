using System.Collections.Generic;
using WindowsInput.Native;
using LaunchPadStreamDeck.API.Enums;

namespace LaunchPadStreamDeck.API.Models
{
    public class KeyboardEvent : IEvent
    {
        public int ProfileNumber { get; set; }
        public string ProfileName { get; set; }
        public string EventName { get; set; }
        public EventType EventType => EventType.Keyboard;
        public int ButtonX { get; set; }
        public int ButtonY { get; set; }

        public IEnumerable<VirtualKeyCode> VirtualKeyModifiers { get; set; }

        public IEnumerable<VirtualKeyCode> VirtualKeyCodes { get; set; }
    }
}
