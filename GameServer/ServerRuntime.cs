using System.Collections;
using System.Collections.Generic;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer {
    /// <summary>
    /// Basic server runtime. Inherit this class to add runtime data to the server.
    /// </summary>
    public class ServerRuntime : IServerRuntime {
        private readonly LinkedList<IEnumerator> _coroutines = new LinkedList<IEnumerator>();
        /// <summary>
        /// Starts a coroutine.
        /// 
        /// The execution of a coroutine can be paused at any point using the <code>yield return null</code> statement. Coroutine will be resumed at the next frame. Coroutines are excellent when modelling behaviour over several frames. Coroutines have virtually no performance overhead. There is no guarantee that coroutines end in the same order that they were started, even if they finish in the same frame.
        /// </summary>
        /// <param name="coroutine">Call result of the coroutine method.</param>
        /// <example><![CDATA[
        /// IEnumerator CoroutineThatWorksXFrames(int x) {
        ///     for (var i = 0; i < x; i++) {
        ///         Debug.Log(i);
        ///         yield return null;
        ///     }
        /// }
        /// ...
        /// runtime.StartCoroutine(CoroutineThatWorksXFrames(10));
        /// ]]></example>
        public Coroutine StartCoroutine(IEnumerator coroutine) {
            if (coroutine.MoveNext()) {
                _coroutines.AddLast(coroutine);
            }
            return default(Coroutine);
        }

        /// <summary>
        /// Updates all coroutines. Should be called once per tick (GameProcessor does it).
        /// </summary>
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
