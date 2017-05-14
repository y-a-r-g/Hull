using System.Collections;
using System.Collections.Generic;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer {
    public class ServerRuntime : IServerRuntime {
        private readonly LinkedList<IEnumerator> _coroutines = new LinkedList<IEnumerator>();
        
        public void StartCoroutine(IEnumerator coroutine) {
            if (coroutine.MoveNext()) {
                _coroutines.AddLast(coroutine);
            }
        }

        public void UpdateCoroutines() {
            var iterator = _coroutines.First;
            while (iterator != null) {
                if (!iterator.Value.MoveNext()) {
                    var copy = iterator;
                    iterator = iterator.Next;
                    _coroutines.Remove(copy);
                }
                else {
                    iterator = iterator.Next;
                }
            }
        }
    }
}
