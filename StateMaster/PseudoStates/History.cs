using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.PseudoStates {

    public enum HistoryKind {
        Deep,
        Shallow
    };

    internal enum HistoryEvent : byte {
        StateAdded, 
        StateRemoved, 
        ParentEntered, 
        ParentExited
    }

    public sealed class History : AbstractStates.Pseudo, IEnumerable<AbstractStates.State> {
        //enum InternalStates : byte {
        //    ParentEnteredHistoryActive,
        //    ParentEnteredHistoryInactive,
        //    ParentExitedHistoryInactive,
        //    ParentExitedHistoryActive
        //}

        //InternalStates m_InternalState = InternalStates.ParentExitedHistoryInactive;

        readonly List<AbstractStates.State> 
            m_History = new List<AbstractStates.State>();
        readonly HistoryKind m_Kind;

        History() {}

        internal History(HistoryKind p_Kind)
        {
            m_Kind = p_Kind;
        }

        AbstractStates.State GetShallowAncestorOrNull(AbstractStates.Atomic p_State)
        {
            AbstractStates.State tSA = p_State;
            while (!ReferenceEquals(tSA, null) && !ReferenceEquals(tSA.Parent, this.Parent))
                tSA = tSA.Parent;
            return tSA;
        }

        AbstractStates.Atomic GetRelevantStateOrNull(AbstractStates.Atomic p_State)
        {
            // never remeber or forget this state or pseudo states
            // TODO: I am not sure about the semantics of "remembering" pseudo states...
            if (ReferenceEquals(this, p_State))
                return null;

            return p_State;
            //switch (m_Kind) {
            //    case HistoryKind.Deep:
            //        return p_State;
            //    case HistoryKind.Shallow:
            //        return GetShallowAncestorOrNull(p_State);
            //    default:
            //        return null;
            //}
        }

        /// <summary>
        /// Remove the state last added to history, if not from parallel substate
        /// other than p_EnteredState
        /// </summary>
        /// <param name="p_EnteredState">State to be added to configuration</param>
        void RemoveLastFromHistory(AbstractStates.Atomic p_EnteredState)
        {
            if (m_History.Count > 0) {
                m_History.RemoveAt(m_History.Count - 1);
            }
        }

        void AddToHistory(AbstractStates.Atomic p_State)
        {
            m_History.Add(p_State);
        }

        void OnParentExited(AbstractStates.Composite p_Parent)
        {
            System.Diagnostics.Debug
                .Assert(m_History.Count != 0, "history is empty!");

            IsHistoryActive = true;
            IsParentEntered = false;
        }

        void OnParentEntered(AbstractStates.Composite p_Parent)
        {
            IsParentEntered = true;
        }

        void OnStatedRemoved(AbstractStates.Composite p_Parent, AbstractStates.Atomic p_State)
        {
            var tS = GetRelevantStateOrNull(p_State);
            if (tS != null) {
                AddToHistory(tS);
            }
        }

        void OnStateAdded(AbstractStates.Composite p_Parent, AbstractStates.Atomic p_State)
        {
            var tS = GetRelevantStateOrNull(p_State);
            if (tS != null) {
                // parent was not entered via history
                if (IsParentEntered) {
                    IsHistoryActive = false;
                    m_History.Clear();
                }
                RemoveLastFromHistory(tS);
            }
        }

        /// <summary>
        /// Handler for the history state machine
        /// </summary>
        /// <param name="p_Event">Event fired from composite state containing this history</param>
        /// <param name="p_Parent">Composite state containing this history</param>
        /// <param name="p_State">Atomic state participating in this history state event</param>
        internal void HandleHistoryEvent(
            HistoryEvent p_Event, 
            AbstractStates.Composite p_Parent, 
            AbstractStates.Atomic p_State)
        {
            switch (p_Event) {
                case HistoryEvent.StateAdded:
                    OnStateAdded(p_Parent, p_State);
                    break;
                case HistoryEvent.StateRemoved:
                    OnStatedRemoved(p_Parent, p_State);
                    break;
                case HistoryEvent.ParentEntered:
                    OnParentEntered(p_Parent);
                    break;
                case HistoryEvent.ParentExited:
                    OnParentExited(p_Parent);
                    break;
                default:
                    throw new ArgumentException("Invalid event for History", "p_Event");
            }
        }

        public HistoryKind Kind
        {
            get { return m_Kind; }
        }

        Boolean IsParentEntered { get; set; }
        public Boolean IsHistoryActive { get; private set; }

        internal override bool TryHandleEvent(
            Event p_Event, 
            AbstractStates.Atomic p_Originator, 
            IPushEventQueue p_Queue)
        {
            // this basically must behave as a choice pseudo state:
            // [!IsActive]/Target
            // [IsActive]/TargetFromHistory
            if (IsHistoryActive) {
                var tHandled = TryHandleTransitFromHistory(p_Event, p_Originator, p_Queue);
                if (tHandled) {
                    IsHistoryActive = false;
                }
                return tHandled;
            } else {
                return base.TryHandleEvent(p_Event, p_Originator, p_Queue);
            }
            //if (m_InternalState != InternalStates.ParentEnteredHistoryActive) {
            //    return base.TryHandleEvent(p_Event, p_Originator, p_Queue);
            //} else {
            //    // get target from history and transit:
            //    return TryHandleTransitFromHistory(p_Event, p_Originator, p_Queue);
            //}
        }

        private bool TryHandleTransitFromHistory(
            Event p_Event, AbstractStates.Atomic p_Originator, IPushEventQueue p_Queue)
        {
            switch (m_History.Count) {
                case 0:
                    return base.TryHandleEvent(p_Event, p_Originator, p_Queue);
                case 1:
                    new Core.Transition() {
                        Kind = TransitionKind.Local,
                        Target = m_History[0]
                    }.Execute(p_Event, this, this, p_Queue);
                    return true;
                default:
                    throw new NotImplementedException("History transition to Parallel state not yet implemented");
                    break;
            }
        }

        #region IEnumerable<State> Members

        public IEnumerator<AbstractStates.State> GetEnumerator()
        {
            return m_History.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }

}
