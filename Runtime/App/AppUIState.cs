// ============================================================================
// NanoServiceFlow.UI - App Shell
// ============================================================================

using Aim4code.NanoServiceFlow;

namespace Aim4code.NanoServiceFlow.UI.App
{
    public class AppUIState : UIRootState
    {
        public ReactiveProperty<float> LoadingProgress { get; } = new(0f);
    }
}
