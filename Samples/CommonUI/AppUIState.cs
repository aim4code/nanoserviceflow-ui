// ============================================================================
// NanoServiceFlow.UI Samples - Common UI
// ============================================================================

using Aim4code.NanoServiceFlow;

namespace Aim4code.NanoServiceFlow.UI.Samples.CommonUI
{
    public class AppUIState : UIRootState
    {
        public ReactiveProperty<float> LoadingProgress { get; } = new(0f);
    }
}
