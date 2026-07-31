// ============================================================================
// NanoServiceFlow.UI - Transitions
// ============================================================================

using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Aim4code.NanoServiceFlow.UI
{
    /// <summary>
    /// Drives an Animator component for transition states, waiting an approximate duration
    /// before completing the async task. Use "Show" and "Hide" trigger parameters.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorUITransition : MonoBehaviour, IUITransition
    {
        [SerializeField]
        private string _showTrigger = "Show";
        
        [SerializeField]
        private string _hideTrigger = "Hide";
        
        [Tooltip("The time in seconds to wait for the animation to play out visually")]
        [SerializeField]
        private float _duration = 0.5f;

        [Tooltip("Advance on unscaled time, so the transition still plays while the game is " +
                 "paused (Time.timeScale = 0). This also drives the Animator's own update mode — " +
                 "waiting out the duration is pointless if the clip it plays is frozen.")]
        [SerializeField]
        private bool _ignoreTimeScale = true;

        private Animator _animator;

        private float DeltaTime => _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            // The wait loop below and the clip itself must agree on a clock: an unscaled wait over
            // a scaled Animator completes on time with nothing having visibly moved.
            _animator.updateMode = _ignoreTimeScale
                ? AnimatorUpdateMode.UnscaledTime
                : AnimatorUpdateMode.Normal;
        }

        public async UniTask PlayShowAsync(CancellationToken ct = default)
        {
            _animator.SetTrigger(_showTrigger);

            float time = 0;
            while (time < _duration && !ct.IsCancellationRequested)
            {
                time += DeltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        public async UniTask PlayHideAsync(CancellationToken ct = default)
        {
            _animator.SetTrigger(_hideTrigger);

            float time = 0;
            while (time < _duration && !ct.IsCancellationRequested)
            {
                time += DeltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }
    }
}
