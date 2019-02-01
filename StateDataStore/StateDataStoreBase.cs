using System;
using System.Collections.Generic;
using System.Text;

namespace StateDataStore
{
    public abstract class StateDataStoreBase : IStateDataStore
    {
        public abstract bool UpdateState(string key, object value);
        public abstract string GetStates(string filter);
        public abstract string GetState(List<string> keys);
        public abstract event EventHandler<StateDataStoreUpdateEventArgs> StateUpdateNotify;
    }
}
