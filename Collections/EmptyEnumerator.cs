using System.Collections;
using System.Collections.Generic;

namespace Hull.Collections {
    public struct EmptyEnumerator<TValue> : IEnumerator<TValue> {
        public void Dispose() { }

        public bool MoveNext() {
            return false;
        }

        public void Reset() { }

        public TValue Current { get; private set; }

        object IEnumerator.Current {
            get { return Current; }
        }
    }
}