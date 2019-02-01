using System;
using System.Collections.Generic;
using System.Text;

namespace StateDataStore
{
    public interface IStateDataStore
    {
        /// <summary>
        /// Notification event to indicate a successful state update
        /// </summary>
        event EventHandler<StateDataStoreUpdateEventArgs> StateUpdateNotify;

        /// <summary>
        /// Updates the state object whose key matches the provided key, to the value specified.
        /// If a state object with the provided key does not exist, creates a state object entry within the Data Store.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool UpdateState(string key, object value);

        /// <summary>
        /// Gets all states within the State Data Store whose key contains the substring 'filter'. 
        /// The states are returned as a string, with each state separated by a newline,
        /// and the key separated from the value by a double colon (::).
        /// Eg.
        /// RoomController:room:ProgramAudioChannel:0 :: laptop
        /// RoomController:room:RoomMode::SinglePresentation
        /// RoomController:room:SourceLevel:laptop:: 60
        /// RoomController:room:SourceLevel:zoom.pc :: 80
        /// RoomController:room:SourceMute:laptop::False
        /// RoomController:room:SourceMute:zoom.pc :: False
        /// RoomController:room:SourceSelect:single::laptop
        /// </summary>
        /// <param name="filter">
        /// The filter substring. Only states whose key contains the filter substring is returned. 
        /// If blank, all states are returned.
        /// </param>
        /// <returns></returns>
        string GetStates(string filter);

        /// <summary>
        /// Matches a list of partial keys to the keys of state objects in the Data Store.
        /// Returns the value of the first state object whose key contains all partial keys provided.
        /// </summary>
        /// <param name="partialKeys">List of partial keys to match state object keys against.</param>
        /// <returns></returns>
        string GetState(List<string> partialKeys);

    }
}
