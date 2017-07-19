using System;
using System.Collections.Generic;
using Hull.GameClient.Interfaces;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Observers {
    /// <summary>
    /// Observes single state property and triggers event when it was changed
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    /// <typeparam name="TProperty">Type of observed property</typeparam>
    public class StatePropertyObserver<TState, TProperty> : IStateObserver<TState>
        where TProperty : IStateProperty
        where TState : State {
        public delegate void ObservedStatePropertyChangedDelegate(TProperty property, TState state);

        /// <summary>
        /// This event is triggered when observed property was changed
        /// </summary>
        public event ObservedStatePropertyChangedDelegate ObservedStatePropertyChanged;

        private readonly ulong _propertyUniqueId;
        private IEnumerable<ulong> _path;

        /// <summary>
        /// Creates an observer for given property
        /// </summary>
        /// <param name="property">Property to observe</param>
        /// <param name="handler">Handler that will be automatically added to ObservedStatePropertyChanged event</param>
        /// <exception cref="ArgumentNullException">Property is null</exception>
        public StatePropertyObserver(TProperty property, ObservedStatePropertyChangedDelegate handler = null) {
            if (property == null) {
                throw new ArgumentNullException("property");
            }
            _propertyUniqueId = property.UniqueId;
            if (handler != null) {
                ObservedStatePropertyChanged += handler;
            }
        }

        public void OnStateChange(TState state) {
            if (_path == null) {
                _path = PropertyFinder.GetPropertyPath(state, _propertyUniqueId);
            }
            var property = PropertyFinder.FindProperty(state, _path);
            if (property.IsModified) {
                PropertyChanged((TProperty)property, state);
            }
        }

        /// <summary>
        /// Called when property was changed. Do not call base implementation when override if you dont need event being triggered
        /// </summary>
        /// <param name="property"></param>
        /// <param name="state"></param>
        protected virtual void PropertyChanged(TProperty property, TState state) {
            if (ObservedStatePropertyChanged != null) {
                ObservedStatePropertyChanged(property, state);
            }
        }
    }
}
