// ============================================================================
// NanoServiceFlow.UI - Quick Start Sample
//
// A tiny bridge from a uGUI Button to the navigation system. Attach it to a
// Button and pick a mode:
//   - OpenScreen: dispatch OpenScreenAction(panelId) — the router figures out
//     which canvas/location the panel lives in.
//   - PopPanel:   dispatch PopPanelAction(rootKey, location) — go back one step
//     in that location's stack.
// This is all navigation ever is: dispatch an action. No SetActive, no Show()/Hide().
// ============================================================================

using UnityEngine;
using UnityEngine.UI;
using Aim4code.NanoServiceFlow;
using Aim4code.NanoServiceFlow.UI;

namespace Aim4code.NanoServiceFlow.UI.Samples.QuickStart
{
    [RequireComponent(typeof(Button))]
    public class NavigationButton : MonoBehaviour
    {
        private enum Mode { OpenScreen, PopPanel }

        [SerializeField] private Mode _mode = Mode.OpenScreen;

        [Header("Open Screen")]
        [Tooltip("Panel id to open (OpenScreen mode).")]
        [SerializeField] private string _panelId;

        [Header("Pop Panel")]
        [Tooltip("Root key of the canvas to pop from (PopPanel mode).")]
        [SerializeField] private string _rootKey = "MenuUI";
        [Tooltip("Location/layer to pop within (PopPanel mode).")]
        [SerializeField] private string _location = "Main";

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (_mode == Mode.OpenScreen)
                ServiceLocator.Dispatch(new OpenScreenAction(_panelId));
            else
                ServiceLocator.Dispatch(new PopPanelAction(_rootKey, _location));
        }
    }
}
