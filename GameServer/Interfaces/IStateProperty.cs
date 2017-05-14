using System.Runtime.Serialization;

namespace Hull.GameServer.Interfaces {
    public interface IStateProperty : ISerializable {
        IStatePropertyContainer Container { get; set; }
        bool IsModified { get; }
        void ForceModify();
    }
}