using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.AbstractStates {
    public abstract class StateMachine : Composite, IEventHandler {
        readonly Core.EventQueue m_Queue = new Core.EventQueue();

        bool HandleCurrentEvent(Event p_CurrentEvent)
        {
            bool tHandled = false;
            foreach (var tState in Configuration.ToArray()) {
                if (tState.IsActive) {
                    tHandled = tState.TryHandleEvent(p_CurrentEvent, tState, m_Queue);
                }
            }
            return tHandled;
        }

        void HandleCompletionEvents()
        {
            while (HandleCurrentEvent(Core.Constants.Create(Core.Constants.InternalEvents.Completion)))
                ;
        }

        #region IEventHandler Members

        public void EnableEventHandling()
        {
            if (IsEnabled)
                return;

            Configuration.Clear();

            try {
                var tInfo = new Core.TransitionInfo {
                    Source = null, 
                    Target = this, 
                    LCA = this, 
                    Kind = TransitionKind.Local
                };
                this.OnEnter(ref tInfo);
                // post completion events
                HandleCompletionEvents();
            } catch (Exception p_Ex) {
                Configuration.Clear();
                throw p_Ex;
            }

            IsEnabled = true;
            this.OnEnabled();
        }

        public void HandleEvent(Event p_Event)
        {
            if (!IsEnabled) {
                OnUnhandledEvent(m_Queue.Pull());
            }

            m_Queue.Push(p_Event);
            if (IsBusy) 
                return;

            IsBusy = true;
            while (!m_Queue.Empty) {
                var tCurrentEvent = m_Queue.Pull();

                if (!HandleCurrentEvent(tCurrentEvent))
                    OnUnhandledEvent(tCurrentEvent);

                HandleCompletionEvents();
            }
            IsBusy = false;
        }

        public void DisableEventHandling()
        {
            this.HandleEvent(Core.Constants.Create(Core.Constants.InternalEvents.Termination));
            var tTerminal = Configuration.FirstOrDefault();
            if (tTerminal != null && tTerminal is InternalStates.Terminal) {
                Configuration.Clear();
                IsEnabled = false;
                this.OnDisabled();
            }
        }

        public event Action<Event> UnhandledEvent;
        public event Action<IEventHandler> Enabled;
        public event Action<IEventHandler> Disabled;

        volatile bool m_IsEnabled = false;
        public bool IsEnabled
        {
            get 
            { 
                return m_IsEnabled; 
            }
            private set 
            { 
                m_IsEnabled = value; 
            }
        }

        volatile bool m_IsBusy = false;
        public bool IsBusy
        {
            get
            {
                return m_IsBusy;
            }
            private set
            {
                m_IsBusy = value;
            }
        }

        #endregion

        protected virtual void OnUnhandledEvent(Event p_Event)
        {
            var tTmp = System.Threading.Interlocked.CompareExchange(ref UnhandledEvent, null, null);
            if (tTmp != null)
                tTmp(p_Event);
        }

        protected virtual void OnEnabled()
        {
            var tTmp = System.Threading.Interlocked.CompareExchange(ref Enabled, null, null);
            if (tTmp != null)
                tTmp(this);
        }

        protected virtual void OnDisabled()
        {
            var tTmp = System.Threading.Interlocked.CompareExchange(ref Disabled, null, null);
            if (tTmp != null)
                tTmp(this);
        }
    }
}
