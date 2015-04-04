using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core.Interfaces {
    public interface IMessage {
        IEvent Event { get; }
        IEventQueue EventQueue { get; }
    } 
}
