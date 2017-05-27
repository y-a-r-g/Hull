using Hull.GameClient.Interfaces;

namespace Hull.GameServer.Interfaces {
    /// <summary>
    /// Every request to Game Server should implement this interface. 
    /// It should be simple struct with request data only.
    /// It should be marked as [Serializable]
    /// <seealso cref="IRequestProcessor{TState,TRuntime}"/>
    /// <seealso cref="IServerConnector{TState}"/>
    /// </summary>
    public interface IRequest { }
}
