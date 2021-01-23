using System;

namespace LaunchPadStreamDeck.API.Classes
{
    public class LaunchpadException : Exception
    {
        public LaunchpadException()
        {
        }

        public LaunchpadException(string message)
            : base(message)
        {
        }
    }
}