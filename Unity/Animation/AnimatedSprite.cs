using System;
using System.Collections;
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
                get { return _sprite.IsPlaying; }
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

        public int? StartFrame;

        public int? EndFrame;

        public IEnumerator Play() {
            return Play(_mode);
        }

        public IEnumerator Play(PlayMode mode) {
            _paused = false;
            _mode = mode;
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

        public void Rewind() {
            Time = 0;
            if ((_frames == null) || (_frames.Length == 0)) {
                return;
            }
            
            var startFrame = Mathf.Max(StartFrame.HasValue ? StartFrame.Value : int.MinValue, 0);
            var endFrame = Mathf.Min(EndFrame.HasValue ? EndFrame.Value : int.MaxValue, _frames.Length - 1);
            if (startFrame > endFrame) {
                startFrame = endFrame;
            }

            var frame = startFrame;
            if (_spriteRenderer.sprite != _frames[frame]) {
                _spriteRenderer.sprite = _frames[frame];
            }
        }

        protected virtual void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void Update() {
            if ((_frames == null) || (_frames.Length == 0) || (_framesPerSecond <= 0) || _paused || Finished) {
                return;
            }

            var startFrame = Mathf.Max(StartFrame.HasValue ? StartFrame.Value : int.MinValue, 0);
            var endFrame = Mathf.Min(EndFrame.HasValue ? EndFrame.Value : int.MaxValue, _frames.Length - 1);
            if (startFrame > endFrame) {
                var t = startFrame;
                startFrame = endFrame;
                endFrame = t;
            }

            Time += UnityEngine.Time.deltaTime;
            var frame = Mathf.RoundToInt(Time * _framesPerSecond);

            bool finished = false;
            switch (_mode) {
                case PlayMode.Once:
                    frame += startFrame;
                    finished = frame > endFrame;
                    frame = Mathf.Min(frame, endFrame);
                    break;
                case PlayMode.Loop:
                    frame = frame % (endFrame - startFrame + 1);
                    frame += startFrame;
                    break;
                case PlayMode.OnceReversed:
                    frame = endFrame - frame;
                    finished = frame < startFrame;
                    frame = Math.Max(frame, startFrame);
                    break;
                case PlayMode.LoopReversed:
                    frame = frame % (endFrame - startFrame + 1);
                    if (frame < 0) {
                        frame += endFrame - startFrame + 1;
                    }
                    frame += startFrame;
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
