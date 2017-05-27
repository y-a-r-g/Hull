using Hull.GameServer.ServerState;

namespace Hull.GameServer.Interfaces {
    /// <summary>
    /// Interface for updater classes. Updaters can modify server state every tick.
    /// </summary>
    /// <typeparam name="TState">Type of the server <see cref="State"/></typeparam>
    /// <typeparam name="TRuntime">Type of the server <see cref="IServerRuntime"/></typeparam>
    public interface IUpdater<TState, TRuntime> where TState : State where TRuntime : IServerRuntime {
        /// <summary>
        /// This method will be called every tick. State modification is allowed here. Client will be notified about all the changes made with state. 
        /// <seealso cref="GameProcessor{TState,TRuntime}.AddUpdater"/>
        /// </summary>
        /// <param name="state">Current server state</param>
        /// <param name="runtime">Server runtime</param>
        /// <param name="dt">Time passed from last tick</param>
        void Update(TState state, TRuntime runtime, float dt);
    }
}
