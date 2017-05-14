using System.Collections.Generic;
using Hull.GameClient.Interfaces;
using Hull.GameServer.ServerState;
using UnityEngine;

namespace Hull.GameClient.Observers {
    public class StateObserver : IStateObserver {
        public float TimeSinceLastUpdate {
            get { return Time.time - _lastUpdateTime; }
        }

        private float _lastUpdateTime;
        private int _pauseObservingCounter;
        private Queue<State> _statesLine = new Queue<State>();

        private readonly LinkedList<IStateObserver> _observers = new LinkedList<IStateObserver>();

        public StateObserver(IServerConnector serverConnector) {
            serverConnector.StateChanged += OnStateChange;
        }

        public void AddStateObserver(IStateObserver stateObserver) {
            _observers.AddLast(stateObserver);
        }

        public void RemoveStateObserver(IStateObserver stateObserver) {
            _observers.Remove(stateObserver);
        }

        public void OnStateChange(State state) {
            if ((_pauseObservingCounter == 0) && (_statesLine.Count == 0)) {
                NotifyObserves(state);
            }
            else {
                _statesLine.Enqueue(state);
            }
        }

        public void PauseObserving() {
            _pauseObservingCounter++;
        }

        public void ResumeObserving() {
            _pauseObservingCounter--;

            if (_pauseObservingCounter == 0) {
                if (_statesLine.Count > 0) {
                    NotifyObserves(_statesLine.Dequeue());
                }
            }
        }

        private void NotifyObserves(State state) {
            _lastUpdateTime = Time.time;

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
