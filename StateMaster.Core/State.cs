using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using StateMaster.Core.Interfaces;

namespace StateMaster.Core {

    public abstract class State : Interfaces.IEventHandler {
        Action m_OnEntry, m_OnExit;

        internal State Parent { get; set; }
        internal Int32 ID { get; set; }
        internal TransitionTable Transitions { get; set; }
        internal Action OnEntry 
        { 
            get { return m_OnEntry; }
            set { m_OnEntry = value; } 
        }
        internal Action OnExit
        {
            get { return m_OnExit; }
            set { m_OnExit = value; }
        }

        internal State() {}

        internal State(Int32 pID, State pParent, TransitionTable pTransitions)
        {
            ID = pID;
            Parent = pParent;
            Transitions = pTransitions;
        }

        #region IEventHandler Members

        public ITransitor HandleEvent(IEvent p_Event)
        {
            return HandleYourself(p_Event, this) ?? ForwardToParent(p_Event, this);
        }

        public virtual void Enter()
        {
            Action tTmp = Interlocked.CompareExchange(ref m_OnEntry, null, null);
            if (tTmp != null) {
                tTmp();
            }
        }

        public virtual void Exit()
        {
            Action tTmp = Interlocked.CompareExchange(ref m_OnExit, null, null);
            if (tTmp != null) {
                tTmp();
            }
        }

        #endregion

        #region Auxiliary methods

        protected ITransitor HandleYourself(IEvent p_Event, State p_Source)
        {
            IEnumerable<ITransition> tTransitions;
            if (Transitions.TryGetTransitionSet(p_Event.ID, out tTransitions)) {
                foreach (var tT in tTransitions) {
                    // transiton enabled?
                    ITransitor tTransitor;
                    if (tT.TryFire(p_Event, p_Source, this, out tTransitor)) {
                        return tTransitor;
                    }
                }
            }
            return null;
        }

        protected ITransitor ForwardToParent(IEvent p_Event, State p_Source)
        {
            for (var tS = this.Parent; tS != null; tS = tS.Parent) {
                ITransitor tTransitor = tS.HandleYourself(p_Event, p_Source);
                if (tTransitor != null)
                    return tTransitor;
            }
            return null;
        }
        #endregion
    }
}
