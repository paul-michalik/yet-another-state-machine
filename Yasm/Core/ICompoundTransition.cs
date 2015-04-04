using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasm.Core {
    public interface ICompoundTransition : IEnumerable<ATransition> {
        IEnumerable<AVertex> Sources { get; }
        IEnumerable<AVertex> Targets { get; }
        AVertex MainSource { get; }
        AVertex MainTarget { get; }
    }
}
