using System.Collections.Generic;

namespace Hull.Collections {
    public interface IImmutableList<TValue> : IEnumerable<TValue> {
        int Length { get; }
    }
}