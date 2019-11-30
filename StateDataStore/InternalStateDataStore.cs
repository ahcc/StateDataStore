using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StateDataStore
{
    public class InternalStateDataStore : IStateDataStore
    {
        private Dictionary<string, string> _states = new Dictionary<string, string>(); // The Data Store Dictionary

        /// <summary>
        /// Notification event to indicate a successful state update
        /// </summary>
        public event EventHandler<StateDataStoreUpdateEventArgs> StateUpdateNotify = delegate { };

        /// <summary>
        /// Connects this room to the StateDataStore database, provided that the correct roomUid and authToken are provided.
        /// </summary>
        /// <param name="roomUid">The Room UID in the database.</param>
        /// <param name="authToken">The token authenticating this room to the StateDataStore service.</param>
        /// <returns>True if successful. False otherwise.</returns>
        public bool Initialize(string roomUid, string authToken)
        {
            return true; // Do nothing in this case, since the StateDataStore is internal and no database connection and authentication is required
        }

        /// <summary>
        /// Performs the actual state update, using a key value pair.
        /// </summary>
        /// <param name="key">The key component of the state key-value pair.</param>
        /// <param name="value">The value component of the state key-value pair.</param>
        /// <returns>If the update operation was successful.</returns>
        public bool UpdateState(string key, object value)
        {
            if (!String.IsNullOrEmpty(key) && value != null)
            {
                if (StateChanged(key, value)) // Only perform the update if the value has changed...
                {
                    _states[key] = value.ToString();
                    NotifyStateUpdated(key, value.ToString()); // Send a notification event that the state update was successful
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets all states within the State Data Store whose key contains the substring 'filter'.
        /// </summary>
        /// <param name="regex">
        /// The regex substring. Only states whose key matches the regex is returned. 
        /// If blank, all states are returned.
        /// </param>
        /// <returns></returns>
        public string GetStates(string regex)
        {
            // Acquire keys and sort them.
            var keys = _states.Keys.ToList();
            keys.Sort(); // Sort in alphabetical order

            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            writer.WriteStartObject();
            writer.WritePropertyName("states");
            writer.WriteStartArray();
            foreach (var key in keys)
            {
                if (String.IsNullOrEmpty(regex) || Regex.Match(key.ToLower(), regex.ToLower()).Success) // ToLower makes the comparison case insensitive
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("guid");
                    writer.WriteValue(key);
                    writer.WritePropertyName("value");
                    writer.WriteValue(_states[key]);
                    writer.WriteEndObject();
                }
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
            return sw.ToString();
        }

        /// <summary>
        /// Gets the value of a state object stored in the Data Store, given the key or substring contained within the key.
        /// </summary>
        /// <param name="partialKey">
        /// A search string to match the state object's key.
        /// </param>
        /// <param name="descriptor">
        /// A search string to match the name of the object's property or sub-property.
        /// Sub-properties are matched as "property:key" (for Dictionary elements) or "property:index" for List elements.
        /// </param>
        /// <returns>The object property / sub-property state, represented as a string.</returns>
        public string GetState(List<string> keys)
        {
            foreach (var kvp in _states)
            {
                if (ContainsAll(kvp.Key, keys))
                {
                    var sw = new StringWriter();
                    var writer = new JsonTextWriter(sw);
                    writer.WriteStartObject();
                    writer.WritePropertyName(kvp.Key);
                    writer.WriteValue(kvp.Value);
                    writer.WriteEndObject();
                    return sw.ToString();
                }
            }
            return null;
        }

        private bool ContainsAll(string target, List<string> matches)
        {
            foreach (var match in matches)
            {
                if (!target.Contains(match))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This checks if the new State value has changed from the current value.
        /// </summary>
        /// <param name="key">The key in the State data store.</param>
        /// <param name="value">The new value.</param>
        /// <returns>True if the state is changed. False otherwise.</returns>
        private bool StateChanged(string key, object value)
        {
            if (_states.TryGetValue(key, out var currentValue))
            {
                if (value.ToString() == currentValue)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Notifies that a state has changed.
        /// </summary>
        /// <param name="key">The key of the state object that has changed.</param>
        /// <param name="value">The value that the state object has changed to.</param>
        private void NotifyStateUpdated(string key, string value)
        {
            StateUpdateNotify(this, new StateDataStoreUpdateEventArgs(key, value));
        }
    }
}
