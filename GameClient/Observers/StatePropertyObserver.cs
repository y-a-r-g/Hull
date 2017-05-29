using System;
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

        private readonly TProperty _property;

        /// <summary>
        /// Creates an observer for given property
        /// </summary>
        /// <param name="property">Property to observe</param>
        /// <exception cref="ArgumentNullException">Property is null</exception>
        public StatePropertyObserver(TProperty property) {
            if (property == null) {
                throw new ArgumentNullException("property");
            }
            _property = property;
        }

        public void OnStateChange(TState state) {
            if (_property.IsModified) {
                PropertyChanged(_property, state);
            }
        }

        /// <summary>
        /// Called when property was changed. Do not call base implementation when override if you dont need event being triggered
        /// </summary>
        /// <param name="property"></param>
        /// <param name="state"></param>
        protected virtual void PropertyChanged(TProperty property, TState state) {
            if (ObservedStatePropertyChanged != null) {
                ObservedStatePropertyChanged(_property, state);
            }
        }
    }
}
