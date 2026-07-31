// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Aim4code.NanoServiceFlow.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        [Header("Identity")]
        [Tooltip("The unique ID used to open this panel (e.g., 'SettingsMenu')")]
        [SerializeField, UIPanelDropdown]
        private string _panelId;

        public string PanelId => _panelId;
        
        [Tooltip("If false, this panel reacts to state but won't register with the global router.")]
        public bool RegisterWithRouter = true;

        private CanvasGroup _canvasGroup;
        private IUITransition _transition;
        
        private UIRootProvider _rootProvider;
        private UILocationProvider _locationProvider;
        
        private IDisposable _stateSub;
        private bool _isVisible;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _transition = GetComponent<IUITransition>();
            
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0f;

            _rootProvider = GetComponentInParent<UIRootProvider>();
            _locationProvider = GetComponentInParent<UILocationProvider>();

            if (_rootProvider == null)
            {
                Debug.LogError($"[UIPanel] Missing UIRootProvider in parent hierarchy of '{PanelId}'!");
                return;
            }

            if (_locationProvider == null)
            {
                Debug.LogError($"[UIPanel] Missing UILocationProvider in parent hierarchy of '{PanelId}'!");
                return;
            }

            if (RegisterWithRouter)
            {
                ServiceLocator.Dispatch(new RegisterUIRouteAction(
                    PanelId, 
                    _rootProvider.RootKey, 
                    _locationProvider.LocationName
                ));
            }
        }

        private void Start()
        {
            if (_rootProvider == null) return;

            var state = _rootProvider.GetState();
            _stateSub = state.ActivePanels.Subscribe(OnStateChanged);
        }

        private async void OnStateChanged(HashSet<string> activePanels)
        {
            bool shouldBeVisible = activePanels.Contains(PanelId);
            if (shouldBeVisible == _isVisible) return;

            _isVisible = shouldBeVisible;

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            var token = this.GetCancellationTokenOnDestroy();

            if (shouldBeVisible)
            {
                // Yield one frame to ensure any hiding panels grab the lock first
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                if (IsSuperseded(shouldBeVisible)) return;

                await _locationProvider.TransitionLock.WaitAsync(token);
                try
                {
                    if (IsSuperseded(shouldBeVisible)) return;

                    if (_transition != null) await _transition.PlayShowAsync(token);
                    else _canvasGroup.alpha = 1f;

                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;
                }
                finally
                {
                    _locationProvider.TransitionLock.Release();
                }
            }
            else
            {
                // Grab the lock instantly
                await _locationProvider.TransitionLock.WaitAsync(token);
                try
                {
                    if (IsSuperseded(shouldBeVisible)) return;

                    if (_transition != null) await _transition.PlayHideAsync(token);
                    else _canvasGroup.alpha = 0f;
                }
                finally
                {
                    _locationProvider.TransitionLock.Release();
                }
            }
        }

        /// <summary>
        /// Has the state moved on while this transition was waiting? There are two awaits before a
        /// transition actually starts — a frame yield, then the location's <c>TransitionLock</c> —
        /// and the panel's state can flip during either.
        ///
        /// Without this check the last transition to *finish* decides the final alpha rather than
        /// the current state, so a show and a hide raised in quick succession can leave the panel
        /// faded in while the router believes it is closed. That combination is unrecoverable from
        /// the UI: the panel is no longer on its location stack, so a Pop finds nothing to remove,
        /// the active set never changes, and this handler is never called again to put it away.
        /// </summary>
        private bool IsSuperseded(bool intent) => _isVisible != intent;

        private void OnDestroy()
        {
            _stateSub?.Dispose();
            
            if (RegisterWithRouter && _rootProvider != null && _locationProvider != null)
            {
                ServiceLocator.Dispatch(new UnregisterUIRouteAction(PanelId));
            }
        }
    }
}