using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaunchPadStreamDeck.API.Classes
{
    public class InvocationChecker
    {
        public long InvokeCount;
        private long maxCount;
        
        public InvocationChecker(long count)
        {
            InvokeCount = 0;
            maxCount = count;
        }

        public void CheckStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent) stateInfo;
            InvokeCount += 1;
            if (InvokeCount == maxCount)
            {
                InvokeCount = 0;
                autoEvent.Set();
            }
        }
    }
}
