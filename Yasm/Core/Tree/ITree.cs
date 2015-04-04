using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasm.Core.Tree {

    public interface INode : IElement {
        IEnumerable<INode> Children { get; }
        INode Parent { get; }
    }

    public interface ITree {
        INode Root { get; }
    }
}
