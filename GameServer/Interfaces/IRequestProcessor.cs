using Hull.GameServer.ServerState;

namespace Hull.GameServer.Interfaces {
    /// <summary>
    /// After Game Server receives a <see cref="IRequest"/> from client, <code>ProcessRequest</code> method of corresponding Request Processor will be called.
    /// <seealso cref="GameProcessor{TState,TRuntime}.RegisterProcessor{TRequest}"/>
    /// </summary>
    /// <typeparam name="TState">Type of the server <see cref="State"/></typeparam>
    /// <typeparam name="TRuntime">Type of the server <see cref="IServerRuntime"/></typeparam>
    /// <typeparam name="TRequest">Processed request type</typeparam>
    public interface IRequestProcessor<in TState, in TRuntime, in TRequest>
        where TState : State 
        where TRuntime : IServerRuntime 
        where TRequest : IRequest {
        /// <summary>
        /// Implementation should contain reaction to the request. State modification is allowed here. Client will be notified about all the changes made with state. 
        /// </summary>
        /// <param name="request">Received request</param>
        /// <param name="player">Player that sent this request</param>
        /// <param name="state">Current server state</param>
        /// <param name="runtime">Server runtime</param>
        void ProcessRequest(TRequest request, IPlayer player, TState state, TRuntime runtime);
    }
}
