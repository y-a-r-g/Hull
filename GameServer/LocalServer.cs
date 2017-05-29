using System;
using Hull.GameClient.Interfaces;
using Hull.GameClient.Observers;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameServer {
    /// <summary>
    /// Local server is used to emulate client-server game localy.
    /// </summary>
    public class LocalServer<TState, TServerRuntime> : IServerConnector<TState>
        where TState : State where TServerRuntime : IServerRuntime {
        private readonly GameProcessor<TState, TServerRuntime> _gameProcessor;

        /// <summary>
        /// Triggered whrn state was changed. Use <seealso cref="StateObserver{TState}"/> to handle it.
        /// </summary>
        public event Action<TState> StateChanged;

        /// <summary>
        /// Creates new Local Server with given Game Processor
        /// </summary>
        /// <param name="gameProcessor"></param>
        /// <exception cref="ArgumentNullException">Game Processor is null</exception>
        public LocalServer(GameProcessor<TState, TServerRuntime> gameProcessor) {
            if (gameProcessor == null) {
                throw new ArgumentNullException("gameProcessor");
            }
            _gameProcessor = gameProcessor;
            gameProcessor.StateChanged += OnStateChange;
        }

        private void OnStateChange(TState state) {
            if (StateChanged != null) {
                StateChanged(state);
            }
        }

        /// <summary>
        /// Use this method to send request to the Game Processor.
        /// </summary>
        /// <param name="request">Reques to the Game Processor</param>
        /// <exception cref="ArgumentNullException">Request is null</exception>
        public void SendRequest(IRequest request) {
            if (request == null) {
                throw new ArgumentNullException("request");
            }
            _gameProcessor.ProcessRequest(request, null);
        }
    }
}
