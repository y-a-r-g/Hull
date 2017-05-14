using System;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Interfaces {
    public interface IServerConnector {
        event Action<State> StateChanged;
        void SendRequest(IRequest request);
    }
}