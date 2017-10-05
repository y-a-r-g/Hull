using Hull.Unity.Pooling;
using UnityEngine;

namespace Hull.Unity.Animation {
    public class AutoStartAnimation : MonoBehaviour {
        public bool HideAutomatically;
        private UnityEngine.Animation _animation;
        
        private void Awake() {
            _animation= GetComponent<UnityEngine.Animation>();
        }

        private void OnEnable() {
            if (_animation) {
                _animation.Rewind();
                _animation.Play();
            }
        }

        private void Update() {
            if (HideAutomatically && _animation && !_animation.isPlaying) {
                Pool.Destroy(this);
            }
        }
    }
}
