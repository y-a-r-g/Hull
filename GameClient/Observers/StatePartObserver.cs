using System;
using Hull.GameClient.Interfaces;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Observers {
    public class StatePartObserver<TStatePart> : IStateObserver where TStatePart : IStatePart {
        private readonly Type _type;

        public delegate void ObservedStatePartChangedDelegate(TStatePart statePart, State state);

        public event ObservedStatePartChangedDelegate ObservedStatePartChanged;

        public StatePartObserver() {
            _type = typeof(TStatePart);
        }

        public void OnStateChange(State state) {
            if (ObservedStatePartChanged != null) {
                var part = (TStatePart)state.GetPart(_type);
                if (part.IsModified) {
                    ObservedStatePartChanged(part, state);
                }
            }
        }
    }
}
