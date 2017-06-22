using System;
using UnityEngine;

namespace Hull.Unity.Animation {
    public class Tweener : CustomYieldInstruction {
        /// <summary>
        /// Delegate for the tweenner function.
        /// </summary>
        /// <param name="progress">Tweener progress value in range [0,1]</param>
        public delegate void TweenerUpdate(float progress);

        /// <summary>
        /// Delegate for the tweenner function. If returns false - tweener will be interrupted.
        /// </summary>
        /// <param name="progress">Tweener progress value in range [0,1]</param>
        public delegate bool InterruptableTweenerUpdate(float progress);

        /// <summary>
        /// Delegate for easing function. Should return progress in range [0,1]
        /// <seealso cref="Easing"/>
        /// </summary>
        /// <param name="currentTime">Current time in seconds.</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="endValue">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        public delegate float EasingFunction(float currentTime, float startValue, float endValue, float duration);

        private readonly float _duration;
        private readonly TweenerUpdate _updateFunction;
        private readonly InterruptableTweenerUpdate _interruptableTweenerUpdate;
        private readonly EasingFunction _easing;
        private float _time;

        /// <summary>
        /// Creates new tweener. Can be used like <code>StartCoroutine(new Tweener(...))</code> or like <code>yield return new Tweener(...)</code> inside coroutines.
        /// </summary>
        /// <param name="duration">Duration of the tweener in seconds</param>
        /// <param name="update">Update function will be called every frame with progress in range [0,1]</param>
        /// <param name="easing">Easing function. <seealso cref="EasingFunction"/>></param>
        public Tweener(float duration, TweenerUpdate update, EasingFunction easing = null) : this(duration, easing) {
            _updateFunction = update;
        }
        /// <summary>
        /// Creates new tweener. Can be used like <code>StartCoroutine(new Tweener(...))</code> or like <code>yield return new Tweener(...)</code> inside coroutines.
        /// </summary>
        /// <param name="duration">Duration of the tweener in seconds</param>
        /// <param name="update">Update function will be called every frame with progress in range [0,1]. If returns false - tweener will be interrupted</param>
        /// <param name="easing">Easing function. <seealso cref="EasingFunction"/>></param>
        public Tweener(float duration, InterruptableTweenerUpdate update, EasingFunction easing = null) : this(duration, easing) {
            _interruptableTweenerUpdate = update;
        }

        private Tweener(float duration, EasingFunction easing) {
            if (duration < 0) {
                throw new ArgumentOutOfRangeException("duration");
            }

            if (easing == null) {
                easing = Easing.Linear;
            }

            _duration = duration;
            _easing = easing;
        }

        public override bool keepWaiting {
            get {
                var currentTime = _easing(_time, 0, 1, _duration);

                if (_updateFunction != null) {
                    _updateFunction(currentTime);
                }
                else if (_interruptableTweenerUpdate != null) {
                    if (!_interruptableTweenerUpdate(currentTime)) {
                        return false;
                    }
                }

                if (_time == _duration) {
                    return false;
                }

                _time = Mathf.Clamp(_time + Time.deltaTime, 0, _duration);

                return true;
            }
        }
    }
}
