using System;
using System.Collections.Generic;
using System.Text;

namespace StateDataStore
{
    public class StateDataStoreUpdateEventArgs : EventArgs
    {
        public string Key { get; private set; } // This is used for Dictionary properties
        public object Value { get; private set; }

        public StateDataStoreUpdateEventArgs(string key, object val)
        {
            Key = key;
            Value = val;
        }
    }
}
