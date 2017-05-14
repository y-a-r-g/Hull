using System;
using Hull.GameClient.Interfaces;

namespace Hull.GameClient {
    public abstract class AbstractRuntime : IClientRuntime {
        private readonly IClientRuntime _parentRuntime;

        public event Action Changed;

        public AbstractRuntime(IClientRuntime parentRuntime) {
            _parentRuntime = parentRuntime;
        }

        public void NotifyChanged() {
            if (Changed != null) {
                Changed();
            }
            if (_parentRuntime != null) {
                _parentRuntime.NotifyChanged();
            }
        }
    }
}