using System;

namespace Hull.GameClient.Interfaces {
    /// <summary>
    /// Interface for the client runtime.
    /// </summary>
    public interface IClientRuntime {
        /// <summary>
        /// Triggered when runtime was changed
        /// </summary>
        event Action Changed;
        
        /// <summary>
        /// Used to notyfy that runtime was changed
        /// </summary>
        void NotifyChanged();
    }
}
