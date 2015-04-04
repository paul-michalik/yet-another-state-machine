using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core {
    internal class Message : IMessage {

        #region IMessage Members

        public Event TriggeringEvent
        {
            get;
            internal set;
        }

        public IPushEventQueue Queue
        {
            get;
            internal set;
        }

        #endregion
    }
}
