using System;
using System.Collections.Generic;
using System.Linq;

namespace Yasm.Builders {
    public sealed class Builder {

        sealed class ManageID {
            /// <summary>
            /// Initial default ID
            /// </summary>
            static readonly Int32 c_InitialID = Int32.MinValue;

            /// <summary>
            /// Set of IDs used by this builder
            /// </summary>
            readonly SortedSet<Int32> m_IDSet = new SortedSet<Int32> { 
                c_InitialID 
            };

            internal void Reset()
            {
                m_IDSet.Clear();
                m_IDSet.Add(c_InitialID);
            }

            internal Int32 Next
            {
                get
                {
                    var tNewID = checked(m_IDSet.Max + 1);
                    m_IDSet.Add(tNewID);
                    return tNewID;
                }
            }

            /// <summary>
            /// Used if user defined ID is requested to replace the default auto generated ID
            /// </summary>
            /// <param name="p_CurrentID"></param>
            /// <param name="p_NewID"></param>
            internal void ReplaceCurrentBy(Int32 p_CurrentID, Int32 p_NewID)
            {
                if (p_CurrentID != p_NewID && p_NewID != c_InitialID && m_IDSet.Contains(p_NewID)) {
                    throw new ArgumentException("ID must be unique", "p_NewID");
                }

                m_IDSet.Remove(p_CurrentID);
                if (p_NewID != c_InitialID)
                    m_IDSet.Add(p_NewID);
            }
        }

        readonly ManageID m_ManageStateID = new ManageID();
        readonly ManageID m_ManageTransID = new ManageID();

        public Builder() {}


    }
}
