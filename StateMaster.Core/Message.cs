using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core {
    internal class Message : Interfaces.IMessage {
        internal Message() { }
   
        #region IMessage Members

        public Interfaces.IEvent Event
        {
            get;
            internal set;
        }

        public Interfaces.IEventQueue EventQueue
        {
            get;
            internal set;
        }

        #endregion
    }
}
