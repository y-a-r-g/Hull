using System;
using System.Reflection;
using Hull.Extensions;
using Hull.GameClient.Interfaces;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Observers {
    public class StatePropertiesObserver<TStatePart> : IStateObserver where TStatePart : AbstractStatePart {
        private readonly ObserveMode _mode;
        private readonly Type _partType;
        private readonly FieldInfo[] _fields;

        public delegate void ObservedStatePropertiesChangedDelegate(TStatePart statePart, State state);

        public event ObservedStatePropertiesChangedDelegate ObservedStatePropertiesChanged;

        public StatePropertiesObserver(string[] propertyNames, ObserveMode mode = ObserveMode.Any) {
            _mode = mode;
            _partType = typeof(TStatePart);
            _fields = propertyNames.Map(pn => _partType.GetField(pn));
        }

        public void OnStateChange(State state) {
            if (ObservedStatePropertiesChanged != null) {
                var part = (TStatePart)state.GetPart(_partType);

                foreach (var fieldInfo in _fields) {
                    var property = (IStateProperty)fieldInfo.GetValue(part);
                    if (property.IsModified) {
                        if (_mode == ObserveMode.Any) {
                            ObservedStatePropertiesChanged(part, state);
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
                    ObservedStatePropertiesChanged(part, state);
                }
            }
        }
    }
}