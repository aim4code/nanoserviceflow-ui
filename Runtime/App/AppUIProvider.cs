// ============================================================================
// NanoServiceFlow.UI - App Shell
// ============================================================================

using UnityEngine;

namespace Aim4code.NanoServiceFlow.UI.App
{
    /// <summary>
    /// Attach this component to a Canvas that spans across scenes (DontDestroyOnLoad).
    /// Typically used for global Loading Screens, Overlays, and persistent Audio players.
    /// </summary>
    public class AppUIProvider : UIRootProvider
    {
        [Header("Routing Overrides")]
        [SerializeField, UILocationDropdown]
        private string _loadingPanelLocation;

        [SerializeField, UIPanelDropdown]
        private string _loadingPanelId;

        public override UIRootState GetState() => ServiceLocator.Get<AppUIState>();

        private void Awake()
        {
            transform.SetParent(null); 
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            ServiceLocator.Dispatch(new ConfigureAppUIAction(
                _loadingPanelLocation, 
                _loadingPanelId
            ));
        }
    }
}
