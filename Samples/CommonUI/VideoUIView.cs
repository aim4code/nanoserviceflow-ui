// ============================================================================
// NanoServiceFlow.UI Samples - Common UI
// ============================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Aim4code.NanoServiceFlow.UI.Samples.CommonUI
{
    /// <summary>
    /// A generic View that listens to a VideoPlayer and dispatches to 
    /// the route specified in the dropdown once the video completes or is skipped.
    /// </summary>
    public class VideoUIView : MonoBehaviour
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

        [Header("References")]
        [SerializeField]
        private VideoPlayer _videoPlayer;
        
        private IDisposable _stateSub;
        private bool _isActive;

        private void Awake()
        {
            if (_videoPlayer != null)
                _videoPlayer.loopPointReached += OnVideoFinished;
        }

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
            _isActive = activePanels.Contains(_panelId);
        }

        private void Update()
        {
            if (_isActive && Input.GetKeyDown(KeyCode.Escape))
            {
                AdvanceToNextScreen();
            }
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
            if (_isActive) 
            {
                AdvanceToNextScreen();
            }
        }

        private void AdvanceToNextScreen()
        {
            _isActive = false; 

            if (_actionType == AdvanceActionType.LoadScene)
                ServiceLocator.Dispatch(new LoadSceneAction(_targetSceneName));
            else
                ServiceLocator.Dispatch(new OpenScreenAction(_nextPanelId));
        }

        private void OnDestroy()
        {
            _stateSub?.Dispose();
            if (_videoPlayer != null)
            {
                _videoPlayer.loopPointReached -= OnVideoFinished;
            }
        }
    }
}
