// ============================================================================
// NanoServiceFlow.UI Samples - Common UI
// ============================================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using Aim4code.NanoServiceFlow;
using Cysharp.Threading.Tasks;

namespace Aim4code.NanoServiceFlow.UI.Samples.CommonUI
{
    public class AppUIService : UIServiceBase<AppUIState>
    {
        public string LoadingPanelLocation { get; set; } = "Overlays";
        public string LoadingPanelId { get; set; } = "LoadingPanel";

        public AppUIService(AppUIState state, string rootKey = "AppUI") : base(state, rootKey) { }

        [Reducer]
        public void OnPrepareSceneLoad(PrepareSceneLoadAction action)
        {
            _state.TargetSceneToLoad = action.SceneName;
            
            // Open the Loading Screen!
            ServiceLocator.Dispatch(new PushPanelAction(_rootKey, LoadingPanelLocation, LoadingPanelId));
        }

        [SideEffect]
        public async void OnLoadSceneAsync(LoadSceneAction action)
        {
            Debug.Log($"[AppUIService] Attempting to load scene: '{action.SceneName}'...");
            
            if (string.IsNullOrEmpty(action.SceneName))
            {
                Debug.LogError("[AppUIService] FAILED: Target Scene Name is empty!");
                return;
            }

            _state.LoadingProgress.Value = 0f;

            var operation = SceneManager.LoadSceneAsync(action.SceneName, LoadSceneMode.Single);
            
            if (operation == null)
            {
                Debug.LogError($"[AppUIService] FAILED: Unity could not find scene '{action.SceneName}'. Double-check Build Settings!");
                return;
            }

            // Prevent the scene from activating instantly so the player actually sees the loading bar
            operation.allowSceneActivation = false;
            
            // Unity's progress stops at 0.9f when allowSceneActivation is false
            while (operation.progress < 0.9f)
            {
                // Normalize the 0.0 -> 0.9 value to a clean 0.0 -> 1.0 for the UI Slider
                _state.LoadingProgress.Value = operation.progress / 0.9f; 
                await UniTask.Yield(); 
            }

            _state.LoadingProgress.Value = 1f;
            
            // Wait half a second for visual polish
            await UniTask.Delay(500);

            // Tell Unity to activate the scene
            Debug.Log($"[AppUIService] '{action.SceneName}' loaded into memory. Activating now!");
            operation.allowSceneActivation = true;
            
            // IMPORTANT: Wait for Unity to actually finish the heavy lifting!
            await UniTask.WaitUntil(() => operation.isDone);

            // Optional: Wait one extra frame so the new scene renders before we reveal it
            await UniTask.Yield();

            Debug.Log("[AppUIService] Scene fully activated! Popping the Loading Panel.");
            
            // safe to animate the UI away
            ServiceLocator.Dispatch(new PopPanelAction(_rootKey, LoadingPanelLocation));
        }
    }
}
