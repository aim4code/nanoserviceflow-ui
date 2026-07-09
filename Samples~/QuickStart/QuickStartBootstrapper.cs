// ============================================================================
// NanoServiceFlow.UI - Quick Start Sample
//
// The whole per-scene pattern in one file: a UI root is just three small types
// (state + service + provider) plus a composition root that registers them and
// opens the first screen. Copy this shape into your own project, renaming
// "MenuUI" to your root key.
// ============================================================================

using UnityEngine;
using Aim4code.NanoServiceFlow;
using Aim4code.NanoServiceFlow.UI;

namespace Aim4code.NanoServiceFlow.UI.Samples.QuickStart
{
    // 1. STATE — one navigation state per canvas/root. Usually empty; the base
    //    class already holds the per-location stacks and the reactive ActivePanels.
    public class MenuUIState : UIRootState { }

    // 2. SERVICE — the reducers live in the base class. All you provide is the
    //    root key this canvas answers to (must match the provider's RootKey).
    public class MenuUIService : UIServiceBase<MenuUIState>
    {
        public MenuUIService(MenuUIState state) : base(state, "MenuUI") { }
    }

    // 3. PROVIDER — attach to the Canvas GameObject. Supplies the state instance
    //    to the UIPanels underneath it. (Set its RootKey to "MenuUI" in the inspector.)
    public class MenuUIProvider : UIRootProvider
    {
        public override UIRootState GetState() => ServiceLocator.Get<MenuUIState>();
    }

    // 4. COMPOSITION ROOT — registers the framework for this scene and opens the
    //    first screen. Runs before other scripts so the router exists by the time
    //    UIPanels register their routes in Awake.
    [DefaultExecutionOrder(-100)]
    public class QuickStartBootstrapper : MonoBehaviour
    {
        [Tooltip("Panel id to open on start. Must match a UIPanel's Panel Id in this scene.")]
        [SerializeField] private string _startingPanelId = "Home Panel";

        private void Awake()
        {
            // Register once. RegisterService is idempotent, so re-entering this
            // scene will not double-register handlers.
            ServiceLocator.RegisterService<UIRouterService>();
            ServiceLocator.RegisterState(new MenuUIState());
            ServiceLocator.RegisterService<MenuUIService>();

            // Phase-2 boot for anything implementing IInitializable.
            ServiceLocator.InitializeAll();
        }

        private void Start()
        {
            // UIPanels registered their routes in Awake; now it is safe to navigate.
            ServiceLocator.Dispatch(new OpenScreenAction(_startingPanelId));
        }
    }
}
