// ============================================================================
// NanoServiceFlow.UI Samples - Common UI
// ============================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Aim4code.NanoServiceFlow.UI.Samples.CommonUI
{
    public class LoadingScreenUIView : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField, UIPanelDropdown]
        private string _panelId;

        [Header("References")]
        [SerializeField]
        private Slider _progressBarSlider;
        
        private IDisposable _stateSub;
        private IDisposable _progressSub;
        private bool _isActive;

        private void Start()
        {
            var provider = GetComponentInParent<UIRootProvider>();
            if (provider == null) return;

            var state = provider.GetState();
            _stateSub = state.ActivePanels.Subscribe(OnStateChanged);

            if (state is AppUIState appState)
            {
                _progressSub = appState.LoadingProgress.Subscribe(OnProgressChanged);
            }
        }

        private void OnStateChanged(HashSet<string> activePanels)
        {
            bool wasActive = _isActive;
            _isActive = activePanels.Contains(_panelId);

            if (!_isActive && wasActive)
            {
                if (_progressBarSlider != null) _progressBarSlider.value = 0f;
            }
        }

        private void OnProgressChanged(float progress)
        {
            if (!_isActive || _progressBarSlider == null) return;
            _progressBarSlider.value = progress;
        }

        private void OnDestroy()
        {
            _stateSub?.Dispose();
            _progressSub?.Dispose();
        }
    }
}
