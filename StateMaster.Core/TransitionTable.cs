using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMaster.Core.Interfaces;

namespace StateMaster.Core {

    public class TransitionTable 
        : IEnumerable<KeyValuePair<Int32, IEnumerable<ITransition>>>
    {
        public class Builder {
            TransitionTable m_Table = new TransitionTable();
            public Builder() {}

            public Builder AddTransition(Int32 pID, ITransition pTransition)
            {
                List<ITransition> tTransitionSet = null;
                if (m_Table.m_Transitions.TryGetValue(pID, out tTransitionSet)) {
                    tTransitionSet.Add(pTransition);
                } else {
                    m_Table.m_Transitions.Add(pID, new List<ITransition>(new ITransition[] { pTransition }));
                }
                return this;
            }

            public Builder AddTransitionSet(Int32 pID, IEnumerable<ITransition> pTransitionSet)
            {
                List<ITransition> tTransitionSet = null;
                if (m_Table.m_Transitions.TryGetValue(pID, out tTransitionSet)) {
                    tTransitionSet.AddRange(pTransitionSet);
                } else {
                    m_Table.m_Transitions.Add(pID, new List<ITransition>(pTransitionSet));
                }
                return this;
            }

            internal TransitionTable Get()
            {

                return m_Table;
            }
        }

        Dictionary<Int32, List<ITransition>> m_Transitions = 
            new Dictionary<int, List<ITransition>>();

        internal bool TryGetTransitionSet(
            Int32 p_MessageID, out IEnumerable<ITransition> p_TransitionSet)
        {
            p_TransitionSet = null;
            List<ITransition> tTransitionSet = null;
            if (m_Transitions.TryGetValue(p_MessageID, out tTransitionSet)) {
                p_TransitionSet = tTransitionSet;
                return true;
            }
            return false;
        }

        #region IEnumerable<KeyValuePair<int,IEnumerable<ITransition>>> Members

        public IEnumerator<KeyValuePair<int, IEnumerable<ITransition>>> GetEnumerator()
        {
            return m_Transitions
                .Select(_1 => new KeyValuePair<int, IEnumerable<ITransition>>(_1.Key, _1.Value))
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
