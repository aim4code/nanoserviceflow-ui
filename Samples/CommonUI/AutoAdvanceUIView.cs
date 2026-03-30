// ============================================================================
// NanoServiceFlow.UI Samples - Common UI
// ============================================================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aim4code.NanoServiceFlow.UI.Samples.CommonUI
{
    public enum AdvanceActionType { OpenScreen, LoadScene }

    /// <summary>
    /// A generic View that waits for a specific duration (or an input) 
    /// and then correctly routes to the next panel defined via dropdown.
    /// Perfect for disclaimers, publisher logos, or static splash screens.
    /// </summary>
    public class AutoAdvanceUIView : MonoBehaviour
    {
        [Header("Routing")]
        [SerializeField, UIPanelDropdown]
        private string _panelId;
        
        [SerializeField]
        private AdvanceActionType _actionType = AdvanceActionType.OpenScreen;

        [SerializeField, UIPanelDropdown]
        private string _nextPanelId;

        [SerializeField]
        private string _targetSceneName;
        
        [Header("Timing")]
        [SerializeField]
        private float _displayDuration = 3.0f;
        
        [SerializeField]
        private bool _allowSkip = true;

        private IDisposable _stateSub;
        private bool _isActive;
        private float _timer;

        private void Start()
        {
            var provider = GetComponentInParent<UIRootProvider>();
            if (provider != null)
            {
                _stateSub = provider.GetState().ActivePanels.Subscribe(OnStateChanged);
            }
        }

        private void OnStateChanged(HashSet<string> activePanels)
        {
            bool wasActive = _isActive;
            _isActive = activePanels.Contains(_panelId);

            if (_isActive && !wasActive)
            {
                _timer = 0f;
            }
        }

        private void Update()
        {
            if (!_isActive) return;

            _timer += Time.deltaTime;

            if (_timer >= _displayDuration || (_allowSkip && Input.anyKeyDown))
            {
                _isActive = false;
                
                if (_actionType == AdvanceActionType.LoadScene)
                    ServiceLocator.Dispatch(new LoadSceneAction(_targetSceneName));
                else
                    ServiceLocator.Dispatch(new OpenScreenAction(_nextPanelId));
            }
        }

        private void OnDestroy()
        {
            _stateSub?.Dispose();
        }
    }
}
