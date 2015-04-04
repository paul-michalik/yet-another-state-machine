using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasm.Core {
    public struct Event : IEquatable<Event>, IComparable<Event> {
        public static bool operator ==(Event p_Left, Event p_Right)
        {
            return p_Left.Equals(p_Right);
        }

        public static bool operator !=(Event p_Left, Event p_Right)
        {
            return !p_Left.Equals(p_Right);
        }

        public static bool operator <(Event p_Left, Event p_Right)
        {
            return p_Left.CompareTo(p_Right) < 0;
        }

        public static bool operator <=(Event p_Left, Event p_Right)
        {
            var tRes = p_Left.CompareTo(p_Right);
            return tRes < 0 || tRes == 0;
        }

        public static bool operator >(Event p_Left, Event p_Right)
        {
            return p_Left.CompareTo(p_Right) > 0;
        }

        public static bool operator >=(Event p_Left, Event p_Right)
        {
            var tRes = p_Left.CompareTo(p_Right);
            return tRes > 0 || tRes == 0;
        }

        public static Event Create<TEventID>(TEventID p_ID)
            where TEventID : IConvertible
        {
            var tID = checked(Convert.ToInt32(p_ID));
            return new Event(tID);
        }

        public static Event Create<TEventID>(TEventID p_ID, params Object[] p_Args)
            where TEventID : IConvertible
        {
            var tID = checked(Convert.ToInt32(p_ID));
            return new Event(tID, p_Args ?? null);
        }

        public static Event Create(Int32 p_ID)
        {
            return new Event(p_ID);
        }

        public static Event Create(Int32 p_ID, params Object[] p_Args)
        {
            return new Event(p_ID, p_Args ?? null);
        }

        readonly Int32 m_TriggerID;
        public Int32 TriggerID
        {
            get
            {
                return m_TriggerID;
            }
        }

        readonly Object[] m_Args;
        public Object[] Args
        {
            get
            {
                return m_Args;
            }
        }

        private Event(Int32 p_ID)
        {
            m_TriggerID = p_ID;
            m_Args = null;
        }

        private Event(Int32 p_ID, object[] p_Args)
        {
            m_TriggerID = p_ID;
            m_Args = p_Args;
        }

        public override int GetHashCode()
        {
            return m_TriggerID;
        }

        public override bool Equals(object p_Other)
        {
            if (p_Other == null)
                return false;

            if (p_Other.GetType() != typeof(Event))
                return false;

            return this.Equals((Event)p_Other);
        }


        #region IEquatable<Event> Members

        public bool Equals(Event p_Other)
        {
            return this.TriggerID == p_Other.TriggerID;
        }

        #endregion

        #region IComparable<Event> Members

        public int CompareTo(Event p_Other)
        {
            return this.TriggerID < p_Other.TriggerID ? -1 : (this.TriggerID == p_Other.TriggerID ? 0 : +1);
        }

        #endregion
    }
}
