using System;
using System.Collections.Generic;
using System.Linq;
using StateMaster.Extensions;

namespace StateMaster.AbstractStates {
    public abstract class State {
        public event Action Enter;
        public event Action Exit;
        public abstract Int32 ID { get; internal set; }
        public abstract Composite Parent { get; internal set; }
        public abstract Core.TransitionTable Transitions { get; internal set; }

        /// <summary>
        /// A vertex V queries this vertex whether the transition p_Query:V->this is part
        /// of a compound transition. If yes, it adds all enabled transitions to the
        /// passed enumerable p_Transitions and return true. If no, it does nothing to
        /// p_Transitions and returns false.
        /// </summary>
        /// <param name="p_Event"></param>
        /// <param name="p_Transitions"></param>
        /// <returns></returns>
        internal virtual Boolean CompoundTransitionQueryIn(
            ref Event p_Event, Core.Transition p_Query, ref IEnumerable<Core.Transition> p_Transitions)
        {
            return false;
        }

        internal virtual Boolean CompoundTransitionsQueryIn(
            ref Event p_Event, Core.Transition p_Query, ref IList<Core.Transition> p_Transitions)
        {
            return false;
        }

        internal virtual Boolean CompoundTransitionQueryOut(
            ref Event p_Event, Core.Transition p_Query, ref IEnumerable<Core.Transition> p_Transitions)
        {
            return false;
        }

        internal virtual Core.ICompoundTransition GetEnabledCompoundTransition(Event p_Event)
        {
            var tE = p_Event;
            foreach (var tT in GetEnabledTransitionSet(p_Event)) {
                IEnumerable<Core.Transition> tTransitions = null;
                if (tT.Target.CompoundTransitionQueryIn(ref tE, tT, ref tTransitions)) {
                    return tTransitions == null ? null : new Core.CompoundTransition(tE, tTransitions);
                } else {
                    return tT;
                }
            }
            return null;
        }

        public bool IsActive { get; internal set; }

        internal bool TryGetTransitionSet(Int32 p_EventID, out IEnumerable<Core.Transition> p_TransitionSet)
        {
            p_TransitionSet = null;
            var tTransitions = Transitions;
            return tTransitions != null ?
                tTransitions.TryGetTransitionSet(p_EventID, out p_TransitionSet) : false;
        }

        internal IEnumerable<Core.Transition> GetTransitionSet(Int32 p_EventID)
        {
            var tTransitions = Transitions;
            return tTransitions != null ?
                tTransitions.GetTransitionSet(p_EventID) : Enumerable.Empty<Core.Transition>();
        }

        internal virtual bool TryGetEnabledTransitionSet(
            Event p_Event, out IEnumerable<Core.Transition> p_EnabledTransitionSet)
        {
            if (TryGetTransitionSet(p_Event.ID, out p_EnabledTransitionSet)) {
                p_EnabledTransitionSet = p_EnabledTransitionSet.Where(pT => pT.CanExecute(p_Event));
                return p_EnabledTransitionSet.FirstOrDefault() != null ? true : false;
            }
            return false;
        }

        internal virtual IEnumerable<Core.Transition> GetEnabledTransitionSet(Event p_Event)
        {
            return GetTransitionSet(p_Event.ID)
                .Where(pT => pT.CanExecute(p_Event));
        }

        internal virtual void OnEnter(ref Core.TransitionInfo p_Info)
        {
            IsActive = true;
            Action tTmp = System.Threading.Interlocked.CompareExchange(ref Enter, null, null);
            if (tTmp != null)
                tTmp();
        }

        internal virtual void OnExit(ref Core.TransitionInfo p_Info)
        {
            IsActive = false;
            Action tTmp = System.Threading.Interlocked.CompareExchange(ref Exit, null, null);
            if (tTmp != null)
                tTmp();
        }

        internal virtual bool TryHandleEvent(Event p_Event, Atomic p_Originator, IPushEventQueue p_Queue)
        {
            return 
                TryHandleYourself(p_Event, p_Originator, p_Queue) || 
                (Parent != null ? Parent.TryHandleEvent(p_Event, p_Originator, p_Queue) : false);
        }

        protected internal bool TryHandleYourself(Event p_Event, Atomic p_Originator, IPushEventQueue p_Queue)
        {
            IEnumerable<Core.Transition> tTransitions;
            if (TryGetEnabledTransitionSet(p_Event, out tTransitions)) {
                foreach (var tT in tTransitions) {
                    tT.Execute(p_Event, p_Originator, this, p_Queue);
                }
                return true;
            }

            //IEnumerable<Core.Transition> tTransitions;
            //if (this.TryGetTransitionSet(p_Event.ID, out tTransitions)) {
            //    foreach (var tT in tTransitions) {
            //        // transiton enabled?
            //        if (tT.CanExecute(p_Event)) {
            //            tT.Execute(p_Event, p_Originator, this, p_Queue);
            //            return true;
            //        }
            //    }
            //}

            return false;
        }
    }
}
