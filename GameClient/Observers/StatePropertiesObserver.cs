using System;
using Hull.GameClient.Interfaces;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Observers {
    /// <summary>
    /// Observes multiple properties and triggers event if they all or any of them (depends on mode) were changed.
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    public class StatePropertiesObserver<TState> : IStateObserver<TState> where TState : State {
        private readonly ObserveMode _mode;
        private readonly IStateProperty[] _properties;

        public delegate void ObservedStatePropertiesChangedDelegate(TState state);

        public event ObservedStatePropertiesChangedDelegate ObservedStatePropertiesChanged;

        /// <summary>
        /// Creates on observer for properties passed as params
        /// </summary>
        /// <param name="mode">Observing mode</param>
        /// <param name="properties"></param>
        public StatePropertiesObserver(ObserveMode mode, params IStateProperty[] properties) 
            : this(properties, mode) { }

        /// <summary>
        /// Creates an observer for given properties with specified mode
        /// </summary>
        /// <param name="properties">Properties to observe</param>
        /// <param name="mode">Observe mode</param>
        /// <exception cref="ArgumentNullException">Properties are null or one of properties is null</exception>
        /// <exception cref="ArgumentException">Properties are empty</exception>
        public StatePropertiesObserver(IStateProperty[] properties, ObserveMode mode = ObserveMode.Any) {
            if (properties == null) {
                throw new ArgumentNullException("properties");
            }
            if (properties.Length == 0) {
                throw new ArgumentException("Properties are empty", "properties");
            }
            foreach (var property in properties) {
                if (property == null) {
                    throw new ArgumentNullException("properties");
                }
            }
            _properties = properties;
            _mode = mode;
        }

        public void OnStateChange(TState state) {
            if (ObservedStatePropertiesChanged != null) {
                foreach (var property in _properties) {
                    if (property.IsModified) {
                        if (_mode == ObserveMode.Any) {
                            ObservedStatePropertiesChanged(state);
                            break;
                        }
                    }
                    else {
                        if (_mode == ObserveMode.All) {
                            return;
                        }
                    }
                }
                if (_mode == ObserveMode.All) {
                    ObservedStatePropertiesChanged(state);
                }
            }
        }
    }
}
