using Hull.GameClient.Interfaces;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Observers {
    public class SimpleStatePropertyObserver<TProperty> : IStateObserver
        where TProperty : IStateProperty {
        public delegate void ObservedSimpleStatePropertyChangedDelegate(
            TProperty property,
            State state);

        public event ObservedSimpleStatePropertyChangedDelegate ObservedSimpleStatePropertyChanged;

        private readonly TProperty _property;

        public SimpleStatePropertyObserver(TProperty property) {
            _property = property;
        }

        public void OnStateChange(State state) {
            if (ObservedSimpleStatePropertyChanged != null) {
                if (_property.IsModified) {
                    ObservedSimpleStatePropertyChanged(_property, state);
                }
            }
        }
    }
}
