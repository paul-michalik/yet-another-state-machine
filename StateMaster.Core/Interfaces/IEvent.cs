using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core.Interfaces {
    public interface IEvent {
        Int32 ID { get; }
    }

    //public interface IEvent<out T> : IEvent {
    //    T Arg { get; }
    //}

    //public interface IEvent<out T1, out T2> : IEvent {
    //    T1 Arg1 { get; }
    //    T2 Arg2 { get; }
    //}

    //public interface IEvent<out T1, out T2, out T3> : IEvent  {
    //    T1 Arg1 { get; }
    //    T2 Arg2 { get; }
    //    T3 Arg3 { get; }
    //}
}
