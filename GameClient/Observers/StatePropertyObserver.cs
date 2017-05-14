using System;
using System.Reflection;
using Hull.GameClient.Interfaces;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Observers {
    public class StatePropertyObserver<TStatePart, TProperty> : IStateObserver
        where TStatePart : AbstractStatePart
        where TProperty : IStateProperty {
        public delegate void ObservedStatePropertyChangedDelegate(
            TProperty property,
            TStatePart statePart,
            State state);

        public event ObservedStatePropertyChangedDelegate ObservedStatePropertyChanged;

        private readonly Type _partType;
        private readonly FieldInfo _fieldInfo;

        public StatePropertyObserver(string propertyName) {
            _partType = typeof(TStatePart);
            _fieldInfo = _partType.GetField(propertyName);
        }

        public void OnStateChange(State state) {
            if (ObservedStatePropertyChanged != null) {
                var part = (TStatePart)state.GetPart(_partType);
                var property = (TProperty)_fieldInfo.GetValue(part);
                if (property.IsModified) {
                    ObservedStatePropertyChanged(property, part, state);
                }
            }
        }
    }
}