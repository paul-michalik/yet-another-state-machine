using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasm.Core {

    public enum ElementKind {
        StateMachine,
        Region,
        State,
        Pseudostate,
        Transition
    }

    public interface IElement {
        Int32 ID { get; }

        ElementKind Kind { get; }
    }
}
