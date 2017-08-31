using Hull.GameClient.Observers;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Interfaces {
    /// <summary>
    /// Used to create sproperty observers. 
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public interface IStateObserver<TState> where TState : State {
        /// <summary>
        /// Will be called every time when state changes if observer added to <see cref="StateObserver{TState}"/>. <seealso cref="StateObserver{TState}.AddStateObserver"/>
        /// </summary>
        /// <param name="state">Changed state</param>
        void OnStateChange(TState state);
    }
}