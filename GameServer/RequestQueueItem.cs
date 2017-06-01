using Hull.GameServer.Interfaces;

namespace Hull.GameServer {
    internal struct RequestQueueItem {
        public IPlayer Player;
        public IRequest Request;
    }
}
