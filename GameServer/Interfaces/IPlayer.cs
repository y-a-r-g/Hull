using Hull.Collections;
using Hull.GameClient.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameServer.Interfaces {
    /// <summary>
    /// Interface for player class. Implementation may store any player-related information.
    /// </summary>
    public interface IPlayer<TState> : IStateObserver<TState> where TState : State {
        /// <summary>
        /// Unique identifier of the player
        /// </summary>
        LinearMapId Id { get; }

        /// <summary>
        /// Called when player was registered in gameProcessor
        /// </summary>
        /// <param name="requestReceiver"></param>
        void OnRegister(IRequestReceiver<TState> requestReceiver);
    }
}