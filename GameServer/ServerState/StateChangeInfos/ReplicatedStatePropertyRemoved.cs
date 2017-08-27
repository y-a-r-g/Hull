using System;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.StateChangeInfos {
    [Serializable]
    public struct ReplicatedStatePropertyRemoved : IStateChangeInfo {
        public ulong PropertyId;
    }
}
