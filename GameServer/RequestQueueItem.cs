using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameServer {
    internal struct RequestQueueItem<TState> where TState : State {
        public IPlayer<TState> Player;
        public IRequest Request;
    }
}
