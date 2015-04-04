using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core {
    internal class EventQueue : IPushPullEventQueue {
        readonly Queue<Event> m_Queue = new Queue<Event>();

        #region IPushPullEventQueue Members

        public Event Pull()
        {
            return m_Queue.Dequeue();
        }

        #endregion

        #region IPushEventQueue Members

        public void Push(Event p_Event)
        {
            m_Queue.Enqueue(p_Event);
        }

        public bool Empty
        {
            get 
            {
                return m_Queue.Count == 0;
            }
        }

        #endregion
    }
}
