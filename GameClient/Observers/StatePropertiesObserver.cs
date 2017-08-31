using System;
using System.Collections.Generic;
using Hull.Extensions;
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
        private readonly IEnumerable<ulong> _propertyUniqueIds;
        private IEnumerable<IEnumerable<ulong>> _paths;

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
        /// <param name="handler">Handler that will be automatically added to ObservedStatePropertyChanged event</param>
        /// <exception cref="ArgumentNullException">Properties are null or one of properties is null</exception>
        /// <exception cref="ArgumentException">Properties are empty</exception>
        public StatePropertiesObserver(IStateProperty[] properties, ObserveMode mode = ObserveMode.Any, ObservedStatePropertiesChangedDelegate handler = null) {
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

            _propertyUniqueIds = properties.Map(p => p.UniqueId);
            _mode = mode;
            if (handler != null) {
                ObservedStatePropertiesChanged += handler;
            }
        }

        public void OnStateChange(TState state) {
            if (_paths == null) {
                _paths = _propertyUniqueIds.Map(id => PropertyFinder.GetPropertyPath(state, id));
            }
            foreach (var path in _paths) {
                var property = PropertyFinder.FindProperty(state, path);
                if (property.IsModified) {
                    if (_mode == ObserveMode.Any) {
                        PropertyChanged(state);
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
                PropertyChanged(state);
            }
        }

        /// <summary>
        /// Called when property was changed. Do not call base implementation when override if you dont need event being triggered
        /// </summary>
        /// <param name="state"></param>
        protected virtual void PropertyChanged(TState state) {
            if (ObservedStatePropertiesChanged != null) {
                ObservedStatePropertiesChanged(state);
            }
        }
    }
}