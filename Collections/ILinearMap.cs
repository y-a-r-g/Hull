using System.Collections.Generic;

namespace Hull.Collections {
    public interface ILinearMap<T> : IEnumerable<KeyValuePair<LinearMapId, T>> {
        LinearMapId FreeId { get; }
        int Count { get; }
        int Capacity { get; }
        void Remove(LinearMapId id);
        bool Contains(LinearMapId id);
        T this[LinearMapId id] { get; set; }
    }
}
