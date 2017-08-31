using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Interfaces {
    public interface IReplicator<TState> where TState : State {
        IReplica<TState> Instantinate(IStateProperty property);
        void Destroy(IReplica<TState> replica);
    }
}