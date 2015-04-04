using System;
using System.Collections.Generic;
using System.Linq;
using StateMaster.Extensions;

namespace StateMaster.PseudoStates {

    public sealed class Initial : AbstractStates.Pseudo {
        internal Initial() : base() {}

        internal override bool CompoundTransitionQueryIn(
            ref Event p_Event, Core.Transition p_Query, ref IEnumerable<Core.Transition> p_Transitions)
        {
            throw new InvalidOperationException(
                String.Format("Cannot query {0}!", GetType().ToString()));
        }

        internal override bool CompoundTransitionsQueryIn(
            ref Event p_Event, Core.Transition p_Query, ref IList<Core.Transition> p_Transitions)
        {
            throw new InvalidOperationException(
                String.Format("Cannot query {0}!", GetType().ToString()));
        }
    }

    public sealed class Terminal : AbstractStates.Pseudo {
        internal Terminal() : base() {}

        internal override bool CompoundTransitionQueryIn(
            ref Event p_Event, Core.Transition p_Query, ref IEnumerable<Core.Transition> p_Transitions)
        {
            return false;
        }

        internal override bool CompoundTransitionsQueryIn(
            ref Event p_Event, Core.Transition p_Query, ref IList<Core.Transition> p_Transitions)
        {
            return false;
        }
    }

    public sealed class Fork : AbstractStates.Pseudo {
        internal Fork() : base() {}

        internal override bool CompoundTransitionsQueryIn(
            ref Event p_Event, Core.Transition p_Query, ref IList<Core.Transition> p_Transitions)
        {
            var tOut = this.Outgoing();
            var tE = p_Event;
            if (tOut.All(pT => pT.CanExecute(tE))) {
                p_Transitions = p_Transitions.CreateIfEmpty().Merge(tOut);
            } else {
                p_Transitions = null;
            }
            return true;
        }

        internal override bool CompoundTransitionQueryIn(
            ref Event p_Event, Core.Transition p_Query, ref IEnumerable<Core.Transition> p_Transitions)
        {
            var tOut = this.Outgoing();
            var tE = p_Event;
            if (tOut.All(pT => pT.CanExecute(tE))) {
                p_Transitions = p_Transitions != null ?
                    p_Transitions.Concat(tOut) : tOut;
            } else {
                p_Transitions = null;
            }
            return true;
        }
    }

    public sealed class Join : AbstractStates.Pseudo {
        internal Join() : base() { }

        private IList<Core.Transition> m_Incoming;

        internal override bool CompoundTransitionsQueryIn(
            ref Event p_Event, Core.Transition p_Query, ref IList<Core.Transition> p_Transitions)
        {
            if (m_Incoming == null) {
                m_Incoming = this.Incoming().ToList();
            }

            Event tE = p_Event;
            if (m_Incoming
                .Where(pT => pT != p_Query)
                .All(pT => pT.CanExecute(tE))) {
                    p_Transitions = p_Transitions.CreateIfEmpty().Merge(m_Incoming);
            } else {
                p_Transitions = null;
            }

            return true;
        } 

        bool QueryIncoming(
            ref Event p_Event, Core.Transition p_Query, ref IEnumerable<Core.Transition> p_Transitions)
        {
            if (m_Incoming == null) {
                m_Incoming = this.Incoming().ToList();
            }

            Event tE = p_Event;
            if (m_Incoming
                .Where(pT => pT != p_Query)
                .All(pT => pT.CanExecute(tE))) {
                p_Transitions = p_Transitions != null ?
                    p_Transitions.Concat(m_Incoming) : m_Incoming;
            } else {
                p_Transitions = null;
            }

            return true;
        }

        internal override bool CompoundTransitionQueryIn(
            ref Event p_Event, Core.Transition p_Query, ref IEnumerable<Core.Transition> p_Transitions)
        {
            IEnumerable<Core.Transition> tIn = null, tOut = null;
            if (QueryIncoming(ref p_Event, p_Query, ref tIn) && QueryOutgoing(ref p_Event, p_Query, ref tOut)) {
                if (tIn != null && tOut != null) {
                    p_Transitions = p_Transitions != null ?
                        p_Transitions.Concat(tIn).Concat(tOut) : tIn.Concat(tOut);
                    return true;
                }
            }
            p_Transitions = null;
            return true;
        }

        bool QueryOutgoing(
            ref Event p_Event, Core.Transition p_Query, ref IEnumerable<Core.Transition> p_Transitions)
        {
            Event tE = p_Event;
            var tOut = this.Outgoing().FirstOrDefault(pT => pT.CanExecute(tE));
            if (tOut != null) {
                p_Transitions = p_Transitions != null ?
                    p_Transitions.Concat(tOut) : tOut;
            } else {
                p_Transitions = null;
            }
            return true;
        }
    }

    public sealed class Junction : AbstractStates.Pseudo {
        internal Junction() : base() {}

        internal override bool CompoundTransitionQueryIn(
            ref Event p_Event, Core.Transition p_Query, ref IEnumerable<Core.Transition> p_Transitions)
        {
            p_Transitions = p_Transitions != null ?
                p_Transitions.Concat(p_Query) : p_Query;

            foreach (var tT in this.Outgoing()) {
                if (tT.CanExecute(p_Event)) {
                    if (!tT.Target.CompoundTransitionQueryIn(ref p_Event, tT, ref p_Transitions)) {
                        p_Transitions = p_Transitions.Concat(tT);
                        break;
                    }
                }
            }
            return true;
        }
    }
}
