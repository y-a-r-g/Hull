using System.Runtime.Serialization;

namespace Hull.GameServer.Interfaces {
    public interface IStateProperty : ISerializable {
        ServerState.State State { set; }
        IStatePropertyContainer Container { set; }
        bool IsModified { get; } 
    }
}
