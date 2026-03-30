// ============================================================================
// NanoServiceFlow.UI Samples - Common UI
// ============================================================================

using Aim4code.NanoServiceFlow;

namespace Aim4code.NanoServiceFlow.UI.Samples.CommonUI
{
    public readonly struct LoadSceneAction : IAction
    {
        public readonly string SceneName;
        public LoadSceneAction(string sceneName) => SceneName = sceneName;
    }

    public readonly struct ConfigureAppUIAction : IAction
    {
        public readonly string LoadingPanelLocation;
        public readonly string LoadingPanelId;

        public ConfigureAppUIAction(string location, string panelId)
        {
            LoadingPanelLocation = location;
            LoadingPanelId = panelId;
        }
    }
}
