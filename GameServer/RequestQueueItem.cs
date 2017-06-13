using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameServer {
    internal struct RequestQueueItem<TState, TRuntime> where TState : State where TRuntime : IServerRuntime {
        public IPlayer<TState, TRuntime> Player;
        public IRequest Request;
    }
}
