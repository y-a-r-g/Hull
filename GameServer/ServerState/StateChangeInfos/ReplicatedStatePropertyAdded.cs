using System;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.StateChangeInfos {
    [Serializable]
    public struct ReplicatedStatePropertyAdded : IStateChangeInfo {
        public ulong PropertyId;
    }
}