// ============================================================================
// NanoServiceFlow.UI Samples - Common UI
// ============================================================================

using Aim4code.NanoServiceFlow;

namespace Aim4code.NanoServiceFlow.UI.Samples.CommonUI
{
    public readonly struct PrepareSceneLoadAction : IAction
    {
        public readonly string SceneName;
        public PrepareSceneLoadAction(string sceneName) => SceneName = sceneName;
    }

    public readonly struct LoadSceneAction : IAction
    {
        public readonly string SceneName;
        public LoadSceneAction(string sceneName) => SceneName = sceneName;
    }
}
