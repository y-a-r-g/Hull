using Hull.Collections;
using Hull.GameServer.ServerState;

namespace Hull.GameServer.Interfaces {
    /// <summary>
    /// Interface for player class. Implementation may store any player-related information.
    /// </summary>
    public interface IPlayer<TState, TServerRuntime> where TState : State where TServerRuntime : IServerRuntime{
        
        /// <summary>
        /// Notifies player that state was changed 
        /// </summary>
        void OnStateChange(TState state);

        /// <summary>
        /// Unique identifier of the player
        /// </summary>
        LinearMapId Id { get; }

        /// <summary>
        /// Called when player was registered in gameProcessor
        /// </summary>
        /// <param name="gameProcessor"></param>
        void OnRegister(GameProcessor<TState, TServerRuntime> gameProcessor);
    }
}
