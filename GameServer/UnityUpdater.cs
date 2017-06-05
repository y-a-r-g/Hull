using UnityEngine;
using UnityEngine.Events;

namespace Hull.GameServer {
    internal class UnityUpdater : MonoBehaviour {
        public UnityAction StartAction;
        public UnityAction FixedUpdateAction;

        private void Start() {
            StartAction();
        }

        private void FixedUpdate() {
            FixedUpdateAction();
        }
    }
}
