using System;
using Hull.GameClient.Interfaces;
using Hull.GameClient.Observers;
using Hull.GameServer.Interfaces;
using Server;
using Shared.State;

namespace Hull.GameServer {
    /// <summary>
    /// Local server is used to emulate client-server game localy.
    /// </summary>
    public class LocalServer : IServerConnector<TVState> {
        private readonly GameProcessor<TVState, TVServerRuntime> _gameProcessor;

        /// <summary>
        /// Triggered whrn state was changed. Use <seealso cref="StateObserver{TState}"/> to handle it.
        /// </summary>
        public event Action<TVState> StateChanged;

        /// <summary>
        /// Creates new Local Server with given Game Processor
        /// </summary>
        /// <param name="gameProcessor"></param>
        /// <exception cref="ArgumentNullException">Game Processor is null</exception>
        public LocalServer(GameProcessor<TVState, TVServerRuntime> gameProcessor) {
            if (gameProcessor == null) {
                throw new ArgumentNullException("gameProcessor");
            }
            _gameProcessor = gameProcessor;
            gameProcessor.StateChanged += OnStateChange;
        }

        private void OnStateChange(TVState state) {
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
