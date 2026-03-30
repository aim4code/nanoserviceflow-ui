// ============================================================================
// NanoServiceFlow.UI Samples - Common UI
// ============================================================================

using System.Threading;
using UnityEngine;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;

namespace Aim4code.NanoServiceFlow.UI.Samples.CommonUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class VideoUITransition : MonoBehaviour, IUITransition
    {
        [SerializeField]
        private VideoPlayer _videoPlayer;
        
        [SerializeField]
        private float _duration = 0.5f;
        
        [SerializeField]
        private float _skipDuration = 0.2f;

        private CanvasGroup _canvasGroup;
        private bool _isSkipping = false;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_videoPlayer != null)
                _videoPlayer.playOnAwake = false;
        }
        
        public void FlagAsSkipped() 
        {
            _isSkipping = true;
        }

        public async UniTask PlayShowAsync(CancellationToken ct = default)
        {
            if (_videoPlayer == null) return;
            _isSkipping = false; 
            
            _videoPlayer.Prepare();
            await UniTask.WaitUntil(() => _videoPlayer.isPrepared, cancellationToken: ct);
            _videoPlayer.time = 0;
            _videoPlayer.Play();

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
            float targetDuration = _isSkipping ? _skipDuration : _duration;
            
            float time = 0;
            while (time < targetDuration && !ct.IsCancellationRequested)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / targetDuration);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
            _canvasGroup.alpha = 0f;

            if (_videoPlayer != null)
                _videoPlayer.Stop();
        }
    }
}
