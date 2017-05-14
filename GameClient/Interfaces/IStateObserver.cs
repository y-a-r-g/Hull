using Hull.GameServer.ServerState;

namespace Hull.GameClient.Interfaces {
    public interface IStateObserver {
        void OnStateChange(State state);
    }
}