using Hull.GameServer.ServerState;

namespace Hull.GameServer.Interfaces {
    public interface IRequestProcessor {
        void ProcessRequest(IRequest request, IPlayer player, State state, IServerRuntime runtime);
    }
}
