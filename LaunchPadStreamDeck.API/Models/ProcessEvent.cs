using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaunchPadStreamDeck.API.Enums;

namespace LaunchPadStreamDeck.API.Models
{
    public class ProcessEvent : IEvent
    {
        public int ProfileNumber { get; set; }
        public string ProfileName { get; set; }
        public string EventName { get; set; }
        public EventType EventType => EventType.Process;
        public int ButtonX { get; set; }
        public int ButtonY { get; set; }
        public string ProcessCommand { get; set; }
    }
}
