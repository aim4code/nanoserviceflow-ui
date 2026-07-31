// ============================================================================
// NanoServiceFlow.UI - Transitions
// ============================================================================

using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Aim4code.NanoServiceFlow.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeUITransition : MonoBehaviour, IUITransition
    {
        [SerializeField]
        private float _duration = 0.5f;

        [Tooltip("Advance on unscaled time, so the transition still plays while the game is " +
                 "paused (Time.timeScale = 0). Leave on unless this fade is meant to freeze " +
                 "with the game — a menu that never finishes fading in is a soft lock.")]
        [SerializeField]
        private bool _ignoreTimeScale = true;

        private CanvasGroup _canvasGroup;

        private float DeltaTime => _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public async UniTask PlayShowAsync(CancellationToken ct = default)
        {
            float time = 0;
            while (time < _duration && !ct.IsCancellationRequested)
            {
                time += DeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / _duration);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
            _canvasGroup.alpha = 1f;
        }

        public async UniTask PlayHideAsync(CancellationToken ct = default)
        {
            float time = 0;
            while (time < _duration && !ct.IsCancellationRequested)
            {
                time += DeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / _duration);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
            _canvasGroup.alpha = 0f;
        }
    }
}
