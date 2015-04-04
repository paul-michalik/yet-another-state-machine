using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core {
    internal sealed class Configuration : IEnumerable<AbstractStates.Atomic> {
        readonly List<AbstractStates.Atomic> m_Configuration =
            new List<AbstractStates.Atomic>();

        internal void Add(AbstractStates.Atomic p_State)
        {
            m_Configuration.Add(p_State);
            //p_State.IsActive = true;
        }

        internal bool Remove(AbstractStates.Atomic p_State)
        {
            //p_State.IsActive = false;
            return m_Configuration.Remove(p_State);
        }

        internal Int32 Count 
        {
            get 
            { 
                return m_Configuration.Count; 
            }
        }

        internal void Clear()
        {
            m_Configuration.Clear();
        }

        #region IEnumerable<Atomic> Members

        public IEnumerator<AbstractStates.Atomic> GetEnumerator()
        {
            return m_Configuration.GetEnumerator();
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
