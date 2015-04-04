using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core {
    
    internal class EventProcessor : Interfaces.IEventProcessor, Interfaces.IEventQueue {

        List<Interfaces.IEventHandler> m_ActiveHandlers;
        readonly Queue<Interfaces.IEvent> m_Queue = new Queue<Interfaces.IEvent>();
        Boolean m_IsProcessing;

        #region IEventProcessor Members

        public void ProcessEvent(Interfaces.IEvent pEvent)
        {
            m_Queue.Enqueue(pEvent);
            
            if (m_IsProcessing)
                return;

            m_IsProcessing = true;
            try {
                do {
                    List<Interfaces.IEventHandler> t_NewHandlers = null;
                    if (TryProcessEvent(m_Queue.Dequeue(), out t_NewHandlers)) {
                        m_ActiveHandlers = t_NewHandlers;
                    } else {
                        // event lost, do something appropriate
                    }
                } while (m_Queue.Count != 0);
            } catch {
                // exception thrown, no state change possible... 
                // clean up queue
                m_Queue.Clear();
            }
            m_IsProcessing = false;
        }

        #endregion

        #region IEventQueue Members

        void Interfaces.IEventQueue.PostEvent(Interfaces.IEvent pEvent)
        {
            m_Queue.Enqueue(pEvent);
        }

        #endregion

        bool TryProcessEvent(Interfaces.IEvent p_Event, out List<Interfaces.IEventHandler> p_NewHandlers) 
        {
            p_NewHandlers = null;
            
            if (m_ActiveHandlers == null)
                return false;

            p_NewHandlers = new List<Interfaces.IEventHandler>();
            foreach (var tH in m_ActiveHandlers) {
                var tTransitor = tH.HandleEvent(p_Event);
                if (tTransitor != null) {
                    p_NewHandlers.Add(tTransitor.Transit(this as Interfaces.IEventQueue));
                }
            }
            return p_NewHandlers.Count > 0;
        }
    }
}
