using System;
using Hull.GameClient.Interfaces;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameServer {
    public class LocalServer : IServerConnector {
        private readonly GameProcessor _gameProcessor;

        public event Action<State> StateChanged;

        public LocalServer(GameProcessor gameProcessor) {
            _gameProcessor = gameProcessor;
            gameProcessor.StateChanged += OnStateChange;
        }

        private void OnStateChange(State state) {
            if (StateChanged != null) {
                StateChanged(state);
            }
        }

        public void SendRequest(IRequest request) {
            _gameProcessor.ProcessRequest(request, null);
        }
    }
}
