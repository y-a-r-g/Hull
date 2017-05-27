using Hull.GameServer.ServerState;

namespace Hull.GameServer.Interfaces {
    /// <summary>
    /// Used to provide additional information about state change.
    /// It should be simple struct with change support data only.
    /// It should be marked as [Serializable]
    /// <seealso cref="State.AddChangeInfo"/>
    /// </summary>
    public interface IStateChangeInfo { }
}
