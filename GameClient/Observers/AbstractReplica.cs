using Hull.GameClient.Interfaces;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;
using UnityEngine;

#if UNITY_5_3_OR_NEWER
namespace Hull.GameClient.Observers {
    public abstract class AbstractReplica<TState> : MonoBehaviour, IReplica<TState> where TState : State {
        private ulong _propertyId;
        private StateObserver<TState> _stateObserver;
        private IStateObserver<TState> _observer;

        public IStateProperty Property {
            get { return PropertyFinder.FindProperty(_stateObserver.LastState, ((StatePropertyObserver<TState, IStateProperty>)_observer).Path); }
        }

        private void BeginObserverving() {
            EndOnserving();
            _observer = new StatePropertyObserver<TState, IStateProperty>(_propertyId, OnChange);
            _stateObserver.AddStateObserver(_observer);
        }

        private void EndOnserving() {
            if (_observer != null) {
                ((StatePropertyObserver<TState, IStateProperty>)_observer).ObservedStatePropertyChanged -= OnChange;
                _stateObserver.RemoveStateObserver(_observer);
                _observer = null;
            }
        }

        public void OnStateChange(TState state) {
            if (_observer != null) {
                _observer.OnStateChange(state);
            }
        }

        public void InitializeReplica(ulong proprtyId, StateObserver<TState> stateObserver) {
            _propertyId = proprtyId;
            _stateObserver = stateObserver;
        }

        public virtual void OnChange(IStateProperty property, TState state) { }

        public virtual void OnAdd(IStateProperty property, TState state) {
            BeginObserverving();
        }

        public virtual void OnRemove(TState state) {
            EndOnserving();
        }
    }
}
#endif