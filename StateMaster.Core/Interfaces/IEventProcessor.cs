using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core.Interfaces {
    public interface IEventProcessor {
        void ProcessEvent(IEvent pEvent);
    }

    public interface IEventQueue {
        void PostEvent(IEvent pEvent);
    }
}
