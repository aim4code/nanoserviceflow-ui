// ============================================================================
// NanoServiceFlow.UI Samples - Common UI
// ============================================================================

using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Aim4code.NanoServiceFlow.UI.Samples.CommonUI
{
    /// <summary>
    /// Automatically interpolates a Material float property (e.g. _Fade, _Radius) over time.
    /// Ensures that an instanced material is created so it doesn't leak into the project asset.
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    public class MaterialPropertyUITransition : MonoBehaviour, IUITransition
    {
        [SerializeField]
        private string _propertyName = "_Fade";
        
        [SerializeField]
        private float _duration = 0.5f;

        [Header("Values")]
        [SerializeField]
        private float _hiddenValue = 0f;
        
        [SerializeField]
        private float _shownValue = 1f;

        private Graphic _graphic;
        private Material _instancedMaterial;

        private void Awake()
        {
            _graphic = GetComponent<Graphic>();
            
            // Create a unique instance of the material so we don't accidentally modify the shared asset
            if (_graphic.material != null)
            {
                _instancedMaterial = new Material(_graphic.material);
                _graphic.material = _instancedMaterial;
                _instancedMaterial.SetFloat(_propertyName, _hiddenValue);
            }
        }

        public async UniTask PlayShowAsync(CancellationToken ct = default)
        {
            if (_instancedMaterial == null) return;

            float time = 0;
            while (time < _duration && !ct.IsCancellationRequested)
            {
                time += Time.deltaTime;
                float currentVal = Mathf.Lerp(_hiddenValue, _shownValue, time / _duration);
                _instancedMaterial.SetFloat(_propertyName, currentVal);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
            _instancedMaterial.SetFloat(_propertyName, _shownValue);
        }

        public async UniTask PlayHideAsync(CancellationToken ct = default)
        {
            if (_instancedMaterial == null) return;

            float time = 0;
            while (time < _duration && !ct.IsCancellationRequested)
            {
                time += Time.deltaTime;
                float currentVal = Mathf.Lerp(_shownValue, _hiddenValue, time / _duration);
                _instancedMaterial.SetFloat(_propertyName, currentVal);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
            _instancedMaterial.SetFloat(_propertyName, _hiddenValue);
        }

        private void OnDestroy()
        {
            if (_instancedMaterial != null)
            {
                Destroy(_instancedMaterial);
            }
        }
    }
}
