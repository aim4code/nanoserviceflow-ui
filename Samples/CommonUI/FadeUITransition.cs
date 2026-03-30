// ============================================================================
// NanoServiceFlow.UI Samples - Common UI
// ============================================================================

using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Aim4code.NanoServiceFlow.UI.Samples.CommonUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeUITransition : MonoBehaviour, IUITransition
    {
        [SerializeField]
        private float _duration = 0.5f;
        
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public async UniTask PlayShowAsync(CancellationToken ct = default)
        {
            float time = 0;
            while (time < _duration && !ct.IsCancellationRequested)
            {
                time += Time.deltaTime;
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
                time += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / _duration);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
            _canvasGroup.alpha = 0f;
        }
    }
}
