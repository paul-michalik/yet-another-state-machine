using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasm.Core {
    internal struct CompoundTransitionQuery {
        readonly Event m_TriggeringEvent;
        readonly IList<ATransition> m_Path;
        readonly ATransition m_Segment;

        internal CompoundTransitionQuery(
            Event p_TriggeringEvent, IList<ATransition> p_Path)
        {
            m_TriggeringEvent = p_TriggeringEvent;
            m_Path = p_Path;
            m_Segment = null;
        }

        internal CompoundTransitionQuery(Event p_TriggeringEvent, ATransition p_Segment) 
            : this(p_TriggeringEvent, null as IList<ATransition>) {}

        internal CompoundTransitionQuery(Event p_TriggeringEvent)
            : this(p_TriggeringEvent, new List<ATransition>()) { }

        internal IEnumerable<ATransition> Path 
        {
            get
            {
                return m_Path ?? (m_Segment ?? Enumerable.Empty<ATransition>());
            }
        }

        internal Event TriggeringEvent
        {
            get
            {
                return m_TriggeringEvent;
            }
        }

        internal void Add(ATransition p_Segment)
        {
            m_Path.Add(p_Segment);
        }

        internal void Remove()
        {
            Remove(1);
        }

        internal void Remove(Int32 p_Count)
        {
            while (m_Path.Count > 0 && p_Count > 0) {
                m_Path.RemoveAt(m_Path.Count - 1);
                p_Count--;
            }
        }
    }
}
