using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMaster {

    public interface IMessage {
        Event TriggeringEvent { get; }
        IPushEventQueue Queue { get; }
    }

    public interface IEventHandler {
        void EnableEventHandling();
        void HandleEvent(Event p_Event);
        void DisableEventHandling();
        
        Boolean IsEnabled { get; }
        Boolean IsBusy { get; }

        event Action<Event> UnhandledEvent;
        event Action<IEventHandler> Enabled;
        event Action<IEventHandler> Disabled;
    }
    
    public interface IEventProcessor {
        void PostEvent(Event p_Event);
        event Action<Event> UnprocessedEvent;
    }

    public interface IPushEventQueue {
        void Push(Event p_Event);
        bool Empty { get; }
    }

    public interface IPushPullEventQueue : IPushEventQueue {
        Event Pull();
    }
}
