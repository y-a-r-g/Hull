using System;

namespace Hull.GameClient.Interfaces {
    public interface IClientRuntime {
        event Action Changed;
        void NotifyChanged();
    }
}