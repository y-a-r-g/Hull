using System.Collections.Generic;
using Hull.GameServer.Interfaces;

namespace Hull.Collections {
    public interface ILinearMap<T> : IEnumerable<KeyValuePair<LinearMapId, T>>, IIndexedAccess<T, LinearMapId> {
        LinearMapId FreeId { get; }
        int Count { get; }
        int Capacity { get; }
        LinearMapId Add(T item);
        void Remove(LinearMapId id);
        bool Contains(LinearMapId id);
        bool TryGetValue(LinearMapId id, out T value);
        T this[LinearMapId id] { get; set; }
    }
}
