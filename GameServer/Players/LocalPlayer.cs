using System;
using Hull.Collections;
using Hull.GameClient.Interfaces;
using Hull.GameClient.Observers;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameServer.Players {
    /// <summary>
    /// Local server is used to emulate client-server game localy.
    /// </summary>
    public class LocalPlayer<TState, TServerRuntime> : IServerConnector<TState>, IPlayer<TState, TServerRuntime>
        where TState : State where TServerRuntime : IServerRuntime {
        private GameProcessor<TState, TServerRuntime> _gameProcessor;
        private readonly LinearMapId _playerId;

        /// <summary>
        /// Triggered whrn state was changed. Use <seealso cref="StateObserver{TState}"/> to handle it.
        /// </summary>
        public event Action<TState> StateChanged;

        /// <summary>
        /// Creates new Local Server with given Game Processor
        /// </summary>
        /// <param name="playerId"></param>
        /// <exception cref="ArgumentNullException">Game Processor is null</exception>
        public LocalPlayer(LinearMapId playerId) {
            _playerId = playerId;
        }

        public virtual void OnStateChange(TState state) {
            if (StateChanged != null) {
                StateChanged(state);
            }
        }

        public LinearMapId Id {
            get { return _playerId; }
        }

        public void OnRegister(GameProcessor<TState, TServerRuntime> gameProcessor) {
            _gameProcessor = gameProcessor;
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
            if (_gameProcessor == null) {
                throw new InvalidOperationException("Player is not registered yet.");
            }
            _gameProcessor.ProcessRequest(request, this);
        }

        /// <summary>
        /// Returns GameRuntime player currently registered in.
        /// </summary>
        public GameProcessor<TState, TServerRuntime> GameProcessor {
            get { return _gameProcessor; }
        }
    }
}
