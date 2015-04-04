using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core {
    public class TransitionTable : IEnumerable<KeyValuePair<Int32, Transition>> {
        Dictionary<Int32, List<Transition>> m_Transitions =
            new Dictionary<int, List<Transition>>();

        internal TransitionTable() {}

        internal IEnumerable<Transition> GetTransitionSet(Int32 p_EventID)
        {
            IEnumerable<Transition> tT = null;
            if (TryGetTransitionSet(p_EventID, out tT)) {
                return tT;
            } else {
                return Enumerable.Empty<Transition>();
            }
        }

        internal bool TryGetTransitionSet(
            Int32 p_EventID, out IEnumerable<Transition> p_TransitionSet)
        {
            p_TransitionSet = null;
            List<Transition> t_TransitionSet = null;
            if (m_Transitions.TryGetValue(p_EventID, out t_TransitionSet)) {
                p_TransitionSet = t_TransitionSet;
                return true;
            }
            return false;
        }

        internal void Add(Int32 p_EventID, Transition p_Transition)
        {
            List<Transition> t_TransitionSet = null;
            if (m_Transitions.TryGetValue(p_EventID, out t_TransitionSet)) {
                t_TransitionSet.Add(p_Transition);
            } else {
                m_Transitions.Add(p_EventID, new List<Transition> {
                    p_Transition
                });
            }
        }

        internal bool Remove(Int32 p_EventID)
        {
            return m_Transitions.Remove(p_EventID);
        }

        internal bool Remove(Int32 p_EventID, Transition p_Transition)
        {
            List<Transition> t_TransitionSet = null;
            if (m_Transitions.TryGetValue(p_EventID, out t_TransitionSet)) {
                return t_TransitionSet.Remove(p_Transition);
            } else {
                return false;
            }
        }

        #region IEnumerable<KeyValuePair<int,Transition>> Members

        public IEnumerator<KeyValuePair<int, Transition>> GetEnumerator()
        {
            return m_Transitions
                .SelectMany(_1 => _1.Value.Select(_2 => new KeyValuePair<Int32, Transition>(_1.Key, _2)))
                .Where(_1 => !(_1.Value.Target is InternalStates.Base))
                .GetEnumerator();
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
