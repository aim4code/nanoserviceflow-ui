// ============================================================================
// NanoServiceFlow.UI Samples - Common UI
// ============================================================================

using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Aim4code.NanoServiceFlow.UI.Samples.CommonUI
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

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public async UniTask PlayShowAsync(CancellationToken ct = default)
        {
            _animator.SetTrigger(_showTrigger);
            
            float time = 0;
            while (time < _duration && !ct.IsCancellationRequested)
            {
                time += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        public async UniTask PlayHideAsync(CancellationToken ct = default)
        {
            _animator.SetTrigger(_hideTrigger);
            
            float time = 0;
            while (time < _duration && !ct.IsCancellationRequested)
            {
                time += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }
    }
}
