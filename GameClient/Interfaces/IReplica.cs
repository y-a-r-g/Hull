using Hull.GameClient.Observers;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Interfaces {
    public interface IReplica<TState> : IStateObserver<TState> where TState : State {
        void InitializeReplica(ulong proprtyId, StateObserver<TState> stateObserver);
        void OnAdd(IStateProperty property, TState state);
        void OnRemove(TState state);
    }
}
