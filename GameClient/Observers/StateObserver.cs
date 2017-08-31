using System;
using System.Collections.Generic;
using Hull.GameClient.Interfaces;
using Hull.GameServer.ServerState;
using Hull.GameServer.ServerState.StateChangeInfos;
using UnityEngine;

namespace Hull.GameClient.Observers {
    /// <summary>
    /// Main state observer. It manages all properties observers.
    /// All notification about state changes are stored in queue and observing can be paused.
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    public class StateObserver<TState> where TState : State {
        /// <summary>
        /// Time passed since last server tick
        /// </summary>
        public float TimeSinceLastUpdate {
            get { return Time.time - _lastUpdateTime; }
        }

        private float _lastUpdateTime;
        private int _pauseObservingCounter;
        private IReplicator<TState> _replicator;
        private Dictionary<ulong, IReplica<TState>> _observedReplicas = new Dictionary<ulong, IReplica<TState>>();
        private readonly Queue<TState> _statesLine = new Queue<TState>();

        /// <summary>
        /// Last observed state. This is the same object that provided to the last called observer.
        /// </summary>
        public TState LastState { get; private set; }

        private readonly LinkedList<IStateObserver<TState>> _observers = new LinkedList<IStateObserver<TState>>();

        /// <summary>
        /// Creates an instance of the state observer.
        /// </summary>
        /// <param name="serverConnector">Server connector used to receive state</param>
        /// <param name="initialState">Initial state for observing</param>
        /// <param name="replicator">Replicator factory</param>
        /// <exception cref="ArgumentNullException">Server Connector is null</exception>
        public StateObserver(IServerConnector<TState> serverConnector, TState initialState, IReplicator<TState> replicator = null) {
            if (serverConnector == null) {
                throw new ArgumentNullException("serverConnector");
            }

            _replicator = replicator;

            LastState = initialState;

            serverConnector.StateChanged += OnStateChange;
        }

        /// <summary>
        /// Adds an observer to the state property. <seealso cref="StatePropertyObserver{TState,TProperty}"/>
        /// Observer will be notified about state change.
        /// </summary>
        /// <param name="stateObserver">Property observer</param>
        /// <exception cref="ArgumentNullException">Property Observer is null</exception>
        public void AddStateObserver(IStateObserver<TState> stateObserver) {
            if (stateObserver == null) {
                throw new ArgumentNullException("stateObserver");
            }

            if (!_observers.Contains(stateObserver)) {
                _observers.AddLast(stateObserver);
            }
        }

        /// <summary>
        /// Removes an observer. Observer will no longer being notified on state changes.
        /// </summary>
        /// <param name="stateObserver"></param>
        /// <exception cref="ArgumentNullException">Property Observer is null</exception>
        public void RemoveStateObserver(IStateObserver<TState> stateObserver) {
            if (stateObserver == null) {
                throw new ArgumentNullException("stateObserver");
            }

            _observers.Remove(stateObserver);
        }

        /// <summary>
        /// Removes all observers
        /// </summary>
        public void RemoveAllObservers() {
            _observers.Clear();
        }

        private void OnStateChange(TState state) {
            if ((_pauseObservingCounter == 0) && (_statesLine.Count == 0)) {
                NotifyObserves(state);
            }
            else {
                _statesLine.Enqueue(state);
            }
        }

        /// <summary>
        /// Pauses all futher observers notification. All received state changes will be held. They will keep order when resumed. 
        /// </summary>
        public void PauseObserving() {
            _pauseObservingCounter++;
        }

        /// <summary>
        /// Continue observing.
        /// </summary>
        public void ResumeObserving() {
            _pauseObservingCounter--;

            if (_pauseObservingCounter == 0) {
                if (_statesLine.Count > 0) {
                    NotifyObserves(_statesLine.Dequeue());
                }
            }
        }

        /// <summary>
        /// Destroys all created replicas
        /// </summary>
        public void DestroyAllReplicas() {
            foreach (var replica in _observedReplicas) {
                replica.Value.OnRemove(LastState);
                _replicator.Destroy(replica.Value);
            }

            _observedReplicas.Clear();
        }

        private void NotifyObserves(TState state) {
            _lastUpdateTime = Time.time;
            LastState = state;

            foreach (var stateChangeInfo in state.ChangeInfo) {
                if (stateChangeInfo is ReplicatedStatePropertyAdded) {
                    var replicatedStateAdded = (ReplicatedStatePropertyAdded)stateChangeInfo;
                    var path = PropertyFinder.GetPropertyPath(state, replicatedStateAdded.PropertyId);
                    var property = PropertyFinder.FindProperty(state, path);
                    var replica = _replicator.Instantinate(property);
                    replica.InitializeReplica(replicatedStateAdded.PropertyId, this);
                    _observedReplicas.Add(replicatedStateAdded.PropertyId, replica);
                    replica.OnAdd(property, state);
                }

                if (stateChangeInfo is ReplicatedStatePropertyRemoved) {
                    var replicatedStateRemoved = (ReplicatedStatePropertyRemoved)stateChangeInfo;
                    var replica = _observedReplicas[replicatedStateRemoved.PropertyId];
                    replica.OnRemove(state);
                    _replicator.Destroy(replica);
                }
            }

            var iterator = _observers.First;
            while (iterator != null) {
                iterator.Value.OnStateChange(state);
                iterator = iterator.Next;
            }

            if (_pauseObservingCounter == 0) {
                if (_statesLine.Count > 0) {
                    NotifyObserves(_statesLine.Dequeue());
                }
            }
        }
    }
}