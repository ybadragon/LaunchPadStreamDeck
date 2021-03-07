using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaunchPadStreamDeck.API.Enums;

namespace LaunchPadStreamDeck.API.Models
{
    public interface IEvent
    {
        int ProfileNumber { get; set; }
        string ProfileName { get; set; }
        string EventName { get; set; }
        EventType EventType { get; }
        int ButtonX { get; set; }
        int ButtonY { get; set; }
    }
}
