using System;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Interfaces {
    /// <summary>
    /// Interface for the bridge between client and server
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    public interface IServerConnector<TState> where TState : State {
        /// <summary>
        /// Should be triggered when state was changed
        /// </summary>
        event Action<TState> StateChanged;
        
        /// <summary>
        /// Used by the client to send requests to the server 
        /// </summary>
        /// <param name="request"></param>
        void SendRequest(IRequest request);
    }
}
