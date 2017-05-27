using System;
using Hull.GameClient.Interfaces;

namespace Hull.GameClient {
    /// <summary>
    /// Simple client runtime with notification system support. Runtime classes can be nested. If nested class notified as changed - parent class will be notified too.
    /// </summary>
    public abstract class AbstractClientRuntime : IClientRuntime {
        private readonly IClientRuntime _parentRuntime;

        public event Action Changed;

        /// <summary>
        /// Creates new runtimw
        /// </summary>
        /// <param name="parentRuntime">Parent runtime instance</param>
        public AbstractClientRuntime(IClientRuntime parentRuntime) {
            _parentRuntime = parentRuntime;
        }

        /// <summary>
        /// Call this mehtod after something was changed in runtime
        /// </summary>
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
