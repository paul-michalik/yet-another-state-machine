using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core.Interfaces {
    public interface ITransitor {
        State Transit(IEventQueue pEventQueue);
    }

    //public interface ITransitor<out T> : ITransitor {
    //    IEvent<T> TriggeringEvent { get; }
    //}

    //public interface ITransitor<out T1, out T2> : ITransitor {
    //    IEvent<T1, T2> TriggeringEvent { get; }
    //}

    //public interface ITransitor<out T1, out T2, out T3> : ITransitor {
    //    IEvent<T1, T2, T3> TriggeringEvent { get; }
    //}
}
