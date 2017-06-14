using Hull.GameServer.ServerState;

namespace Hull.GameServer.Interfaces {
    public interface IRequestReceiver<TState> where TState : State {
        void ProcessRequest(IRequest request, IPlayer<TState> player);
    }
}
