using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMaster.Extensions;
using StateMaster.Core;

namespace StateMaster.Extensions {

    public static class TransitionExtensions {

        public static IList<T> CreateIfEmpty<T>(this IList<T> p_List)
        {
            return p_List == null ? new List<T>() : p_List;
        }

        internal static IList<Transition> Merge(this IList<Transition> p_List, IEnumerable<Transition> p_Addend)
        {
            foreach (var tE in p_Addend) {
                p_List.Add(tE);
            }
            return p_List;
        }

        struct CompoundTransitionInfo {
            readonly Event m_TriggeringEvent;
            List<Transition> m_Segments;

            internal CompoundTransitionInfo(Event p_TriggeringEvent)
            {
                m_TriggeringEvent = p_TriggeringEvent;
                m_Segments = null;
            }

            internal Event TriggeringEvent 
            { 
                get 
                { 
                    return m_TriggeringEvent;
                }
            }

            internal IEnumerable<Transition> Segments 
            { 
                get 
                {
                    return m_Segments;
                }
            }

            internal void Add(Transition p_Transition)
            {
                if (m_Segments == null) {
                    m_Segments = new List<Transition>();
                }
                m_Segments.Add(p_Transition);
            }

            internal void Remove()
            {
                if (m_Segments != null && m_Segments.Count > 0) {
                    m_Segments.RemoveAt(m_Segments.Count - 1);
                }
            }
        }

        private static bool TraverseEnabledCompoundForwardRec(
            Transition p_Query, ref CompoundTransitionInfo p_Info)
        {
            return false;
        }

        public static IEnumerable<Transition> TraverseEnabledCompoundForward(
            this AbstractStates.State p_Source, ref Event p_TriggeringEvent)
        {
            foreach (var tT in p_Source.GetEnabledTransitionSet(p_TriggeringEvent)) {
                var tInfo = new CompoundTransitionInfo(p_TriggeringEvent);
                if (TraverseEnabledCompoundForwardRec(tT, ref tInfo)) {
                    if (tInfo.Segments != null) {
                        return tInfo.Segments;
                    }
                } else {
                    return tT;
                }
            }
            return null;
        }

        public static IEnumerable<Transition> TraverseCompoundForward(
            this Transition p_Transition, IEnumerable<Type> p_HeadPseudoTypes)
        {
            yield return p_Transition;

            var tTarget = p_Transition.Target;
            if (p_HeadPseudoTypes.Any(pT => pT == tTarget.GetType())) {
                var tTransitions = tTarget.Transitions;
                if (tTransitions != null) {
                    foreach (var tT in tTransitions
                        .SelectMany(_ => _.Value.TraverseCompoundForward(p_HeadPseudoTypes))) 
                    {
                        yield return tT;
                    }
                }
            }
        }

        public static IEnumerable<Transition> TraverseCompoundForward(this Transition p_Transition)
        {
            yield return p_Transition;

            var tTarget = p_Transition.Target;
            if (tTarget is AbstractStates.Pseudo) {
                var tTransitions = tTarget.Transitions;
                if (tTransitions != null) {
                    foreach (var tT in tTransitions.SelectMany(_ => _.Value.TraverseCompoundForward())) {
                        yield return tT;
                    }
                }
            }
        }

        static IEnumerable<Transition> TraverseCompoundBackward(
            this Transition p_Transition, IEnumerable<Type> p_TailPseudoTypes)
        {
            var tSource = p_Transition.Source;
            if (p_TailPseudoTypes.Any(pT => pT == tSource.GetType())) {
            }
            yield break;
        }

        static IEnumerable<Transition> TraverseCompoundBackward(this Transition p_Transition)
        {
            yield break;
        }
    }
}

namespace StateMaster.Core {
    public class CompoundTransition : ICompoundTransition {

        readonly IEnumerable<Transition> m_Transitions;
        readonly Event m_TriggeringEvent;

        internal CompoundTransition(Event p_TriggeringEvent)
        {
            m_TriggeringEvent = p_TriggeringEvent;
        }

        internal CompoundTransition(Event p_Event, IEnumerable<Transition> p_Transitions)
        {
            this.m_TriggeringEvent = p_Event;
            this.m_Transitions = p_Transitions;
        }

        #region ICompoundTransition Members

        public AbstractStates.State MainSource
        {
            get
            {
                return LCASearch.FindLCA(Sources);
            }
        }

        public AbstractStates.State MainTarget
        {
            get
            {
                return LCASearch.FindLCA(Targets);
            }
        }

        public IEnumerable<AbstractStates.State> Sources
        {
            get
            {
                return m_Transitions
                    .Where(pT => pT.Source is AbstractStates.Simple)
                    .Select(pT => pT.Source);
            }
        }

        public IEnumerable<AbstractStates.State> Targets
        {
            get
            {
                return m_Transitions
                    .Where(pT => pT.Target is AbstractStates.Simple)
                    .Select(pT => pT.Target);
            }
        }

        public Event TriggeringEvent
        {
            get { return m_TriggeringEvent; }
        }

        public void Execute(ref Event _)
        {
            Event tE = m_TriggeringEvent;
            foreach (var tT in m_Transitions)
                tT.Execute(ref tE);
        }

        #endregion

        #region IEnumerable<Transition> Members

        public IEnumerator<Transition> GetEnumerator()
        {
            return m_Transitions.GetEnumerator();
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