using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core {
    internal class Transitor : Interfaces.ITransitor {

        internal Interfaces.IEvent TriggeringEvent {  get; set; }
        internal IEnumerable<State> Source { get; set; }
        internal IEnumerable<State> Target { get; set; }
        internal Action<Interfaces.IMessage> Action { get; set; }

        public Transitor() {}

        #region ITransitor Members

        public State Transit(Interfaces.IEventQueue p_EventQueue)
        {
            // handle exit actions
            foreach (var tS in Source) {
                tS.Exit();
            }
            if (Action != null) {
                Action(new Message {
                    Event = TriggeringEvent,
                    EventQueue = p_EventQueue
                });
            }

            State tTarget = null;
            // handle enter actions
            foreach (var tS in Target) {
                tTarget = tS;
                tS.Enter();
            }
            return tTarget;
        }

        #endregion
    }
}
