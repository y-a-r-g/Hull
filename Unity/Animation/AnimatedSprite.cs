using UnityEngine;

namespace Hull.Unity.Animation {
    [RequireComponent(typeof(SpriteRenderer))]
    public class AnimatedSprite : MonoBehaviour {
        private class WaitAnimationFinishedYeldInstruction : CustomYieldInstruction {
            private readonly AnimatedSprite _sprite;

            public WaitAnimationFinishedYeldInstruction(AnimatedSprite sprite) {
                _sprite = sprite;
            }

            public override bool keepWaiting {
                get { return !_sprite.IsPlaying; }
            }
        }

        public enum PlayMode {
            Once,
            Loop,
            OnceReversed,
            LoopReversed
        }

        [SerializeField] private float _framesPerSecond = 30;
        [SerializeField] private Sprite[] _frames;
        [SerializeField] private bool _playAutomatically = true;
        [SerializeField] private PlayMode _mode = PlayMode.Once;

        private bool _paused = true;
        protected bool Finished;
        protected float Time;
        private SpriteRenderer _spriteRenderer;
        private CustomYieldInstruction _waitInstruction;

        public float FramesPerSecond {
            get { return _framesPerSecond; }
            set { _framesPerSecond = value; }
        }

        public Sprite[] Frames {
            get { return _frames; }
            set { _frames = value; }
        }

        public bool PlayAutomatically {
            get { return _playAutomatically; }
            set { _playAutomatically = value; }
        }

        public PlayMode Mode {
            get { return _mode; }
            set { _mode = value; }
        }

        public bool Paused {
            get { return _paused; }
            set { _paused = value; }
        }

        public bool IsPlaying {
            get { return !Finished; }
        }

        public CustomYieldInstruction Play() {
            return Play(_mode);
        }

        public CustomYieldInstruction Play(PlayMode mode) {
            _paused = false;
            Time = 0;
            Finished = false;

            if (_waitInstruction == null) {
                _waitInstruction = new WaitAnimationFinishedYeldInstruction(this);
            }
            return _waitInstruction;
        }

        protected virtual void Start() {
            if (_playAutomatically) {
                Play();
            }
        }

        public void Stop() {
            _paused = true;
            Finished = true;
        }

        protected virtual void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void Update() {
            if ((_frames == null) || (_frames.Length == 0) || (_framesPerSecond <= 0) || _paused || Finished) {
                return;
            }

            Time += UnityEngine.Time.deltaTime;
            var frame = Mathf.RoundToInt(Time * _framesPerSecond);
            if ((_mode == PlayMode.LoopReversed) || (_mode == PlayMode.OnceReversed)) {
                frame = _frames.Length - frame - 1;
            }

            bool finished = false;
            switch (_mode) {
                case PlayMode.Once:
                    finished = frame >= _frames.Length;
                    frame = Mathf.Min(frame, _frames.Length);
                    break;
                case PlayMode.Loop:
                    frame = frame % _frames.Length;
                    break;
                case PlayMode.OnceReversed:
                    finished = frame < 0;
                    frame = 0;
                    break;
                case PlayMode.LoopReversed:
                    frame = frame % _frames.Length;
                    if (frame < 0) {
                        frame += _frames.Length;
                    }
                    break;
            }

            if (finished) {
                Stop();
            }

            if (_spriteRenderer.sprite != _frames[frame]) {
                _spriteRenderer.sprite = _frames[frame];
            }
        }
    }
}
