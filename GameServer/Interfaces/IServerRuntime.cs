using System.Collections;
using System.Collections.Generic;
using Hull.GameServer.ServerState;

namespace Hull.GameServer.Interfaces {
    /// <summary>
    /// Server runtime used to store any information that should not be stored in the state and should not be derived to the client.
    /// Server runtime is shared between Request Processors and Updaterd.  
    /// <seealso cref="ServerRuntime"/>
    /// </summary>
    public interface IServerRuntime<TState> where TState : State {
        /// <summary>
        /// New coroutine should be created when this method was called.
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns>Identifier of the coroutine</returns>
        Coroutine StartCoroutine(IEnumerator coroutine);

        /// <summary>
        /// This method will be called by GameProcessor when coroutines should be updated.
        /// </summary>
        void UpdateCoroutines();
        
        IEnumerable<IPlayer<TState>> Players { get; }
    }
}
