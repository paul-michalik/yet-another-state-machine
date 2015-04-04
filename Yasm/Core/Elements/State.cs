using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasm.Core {
    public sealed class State : Core.AVertex {
        internal State(Core.ElementKind p_Kind) : base(p_Kind) {}

        IList<Region> m_Regions;
        internal IList<Region> Regions
        {
            get
            {
                return m_Regions;
            }
            set
            {
                m_Regions = value;
            }
        }

        public sealed override IEnumerable<Tree.INode> Children
        {
            get
            {
                return m_Regions ?? Enumerable.Empty<Region>();
            }
        }
    }
}
