using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core.Interfaces {
    public interface IEventHandler {
        ITransitor HandleEvent(IEvent pEvent);
        void Enter();
        void Exit();
    }
}
