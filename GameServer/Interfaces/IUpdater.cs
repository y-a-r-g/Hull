using Hull.GameServer.ServerState;

namespace Hull.GameServer.Interfaces {
    public interface IUpdater {
        void Update(State state, IServerRuntime runtime, float dt);
    }
}
