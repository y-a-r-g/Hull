
using System.Collections;

namespace Hull.GameServer.Interfaces {
    public interface IServerRuntime {
        void StartCoroutine(IEnumerator coroutine);
        void UpdateCoroutines();
    }
}
