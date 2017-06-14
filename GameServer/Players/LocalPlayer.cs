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
    public class LocalPlayer<TState> : IServerConnector<TState>, IPlayer<TState>
        where TState : State  {
        private IRequestReceiver<TState> _requestReceiver;
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

        public void OnRegister(IRequestReceiver<TState> requestReceiver) {
            _requestReceiver = requestReceiver;
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
            if (_requestReceiver == null) {
                throw new InvalidOperationException("Player is not registered yet.");
            }
            _requestReceiver.ProcessRequest(request, this);
        }

        /// <summary>
        /// Returns <see cref="IRequestReceiver{TState}"/>  player currently registered in.
        /// </summary>
        public IRequestReceiver<TState> RequestReceiver {
            get { return _requestReceiver; }
        }
    }
}
