using System;
using System.Reflection;
using Hull.GameClient.Interfaces;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Observers {
    public class IndexedStatePropertyObserver<TStatePart, TValue, TIndex> : IStateObserver
        where TValue : IStateProperty {
        private readonly TIndex _index;
        private readonly Type _partType;
        private readonly FieldInfo _fieldInfo;

        public delegate void ObservedIndexedStatePropertyChangedDelegate(
            TValue property,
            IIndexedAccess<TValue, TIndex> propertyContainer,
            State state);

        public event ObservedIndexedStatePropertyChangedDelegate ObservedIndexedStatePropertyChanged;

        public IndexedStatePropertyObserver(string propertyName, TIndex index) {
            _index = index;
            _partType = typeof(TStatePart);
            _fieldInfo = _partType.GetField(propertyName);
        }

        public void OnStateChange(State state) {
            if (ObservedIndexedStatePropertyChanged != null) {
                var part = state.GetPart(_partType);
                if (part.IsModified) {
                    var property = _fieldInfo.GetValue(part);
                    if (((IStateProperty)property).IsModified) {
                        var indexed = (IIndexedAccess<TValue, TIndex>)property;
                        var value = indexed[_index];
                        if (value.IsModified) {
                            ObservedIndexedStatePropertyChanged(value, indexed, state);
                        }
                    }
                }
            }
        }
    }
}
