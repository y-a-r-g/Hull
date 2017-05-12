using System;
using Hull.GameClient.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Observers {
    public class StatePartsObserver : IStateObserver {
        private readonly Type[] _types;
        private readonly ObserveMode _mode;

        public delegate void ObservedStatePartsChangedDelegate(State state);

        public event ObservedStatePartsChangedDelegate ObservedStatePartsChanged;

        public StatePartsObserver(Type[] types, ObserveMode mode = ObserveMode.Any) {
            _types = types;
            _mode = mode;
        }

        public void OnStateChange(State state) {
            if (ObservedStatePartsChanged != null) {
                foreach (var type in _types) {
                    var part = state.GetPart(type);
                    if (part.IsModified) {
                        if (_mode == ObserveMode.Any) {
                            ObservedStatePartsChanged(state);
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
                    ObservedStatePartsChanged(state);
                }
            }
        }
    }
}
