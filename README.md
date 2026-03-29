# NanoServiceFlow.UI

[![CI](https://github.com/aim4code/nanoserviceflow-ui/actions/workflows/ci.yml/badge.svg)](https://github.com/aim4code/nanoserviceflow-ui/actions)
[![openupm](https://img.shields.io/npm/v/com.aim4code.nanoserviceflow.ui?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.aim4code.nanoserviceflow.ui/)
[![](https://img.shields.io/badge/Unity-2022.3+-57b9d3.svg?style=flat&logo=unity)](https://unity.com/)
![last-commit](https://img.shields.io/github/last-commit/aim4code/nanoserviceflow)
![open-issues](https://img.shields.io/github/issues/aim4code/nanoserviceflow)

> **A reactive, event-driven UI framework for Unity built on NanoServiceFlow. It features hierarchy-based auto-routing, async UniTask transitions, and multi-canvas support.**

NanoServiceFlow.UI extends the core event-driven architecture to the presentation layer. It completely decouples your UI visuals (MonoBehaviours) from your UI logic (Services) and UI data (State), providing a blazing-fast, GC-friendly navigation system.

> [!NOTE]
> **A Note on Dependencies:** Unlike the core NanoServiceFlow package which is strictly zero-dependency, this UI framework relies on **[UniTask](https://github.com/Cysharp/UniTask)**. This is a deliberate choice to ensure high-performance, zero-allocation asynchronous visual transitions without relying on Unity's legacy Coroutines.

## Key Features

* **Hierarchy-Based Auto-Discovery:** Designers build UI naturally in the Unity Canvas. `UIPanel` components automatically look up their parent hierarchy to figure out which Canvas and Layer they belong to and register themselves with the global router.
* **Keyed Multi-System Support:** Easily support additive scenes or multiple Canvases (e.g., `GlobalUI`, `GameHUD`, `WorldSpaceUI`) with isolated navigation stacks.
* **Async/Await Workflow:** Fully integrated with `UniTask`. Safely await complex UI transitions (like screen fades) before proceeding with game logic. Input is safely guarded during animations.
* **Designer-Friendly & Zero Magic Strings:** Designers never touch C# code. Routing configurations are managed in a centralized `UIRoutingDatabase` ScriptableObject. Components like `UIPanel` use custom Inspector Dropdowns to select `PanelId`, `Location`, and `RootKey`, completely eliminating typo-prone magic strings!

## Installation

### Option 1: Install via OpenUPM (Recommended)

The package is available on the [OpenUPM](https://openupm.com/) registry. The easiest way to install it is via the `openupm-cli`:

```bash
openupm add com.aim4code.nanoserviceflow.ui
```

Alternatively, you can manually add the scoped registry to your `Packages/manifest.json`:

```json
"scopedRegistries": [
  {
    "name": "package.openupm.com",
    "url": "[https://package.openupm.com](https://package.openupm.com)",
    "scopes": [
      "com.aim4code",
      "com.cysharp"
    ]
  }
],
"dependencies": {
  "com.aim4code.nanoserviceflow.ui": "0.1.0"
}
```

### Option 2: Install via Git URL

You can also install the package directly from GitHub. Add the following dependency to your `Packages/manifest.json`:

```json
"dependencies": {
  "com.aim4code.nanoserviceflow": "[https://github.com/aim4code/nanoserviceflow.git#v0.2.1](https://github.com/aim4code/nanoserviceflow.git#v0.2.1)",
  "com.aim4code.nanoserviceflow.ui": "[https://github.com/aim4code/nanoserviceflow-ui.git#v0.1.0](https://github.com/aim4code/nanoserviceflow-ui.git#v0.1.0)",
  "com.cysharp.unitask": "[https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask](https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask)"
}
```

> [!IMPORTANT]
> **Version Determinism:** Notice the `#v0.1.0` at the end of the URL. If you omit the version tag, Unity will resolve the dependency using the latest commit on the default branch at the time of checkout. As the branch updates, this can lead to team members having different versions of the package installed, breaking version determinism. Always lock your Git dependencies to a specific release tag.

## Quick Start

### 1. Define Pure Data (State) & Services

Create a concrete State for your specific UI Canvas, and a Provider to attach to the root GameObject.

```csharp
using Aim4code.NanoServiceFlow;
using Aim4code.NanoServiceFlow.UI;

// 1. The Data
public class MainUIState : UIRootState { }

// 2. The Logic
public class MainUIService : UIServiceBase<MainUIState> 
{
    public MainUIService(MainUIState state) : base(state, "MainUI") { }
}

// 3. The Unity Component (Attach to your Canvas)
public class MainUIProvider : UIRootProvider
{
    // Note: RootKey is configured via the Inspector Dropdown on the UIRootProvider component.
    public override UIRootState GetState() => ServiceLocator.Get<MainUIState>();
}
```

### 2. Setup the View (Unity Hierarchy)

Designers build out the canvas hierarchy in the Unity Editor using the provided base components. They select their routing keys from Dropdowns populated by your central `UIRoutingDatabase`. The framework discovers them automatically:

* **Canvas** `[MainUIProvider: RootKey Dropdown="MainUI"]`
  * **MainStack GameObject** `[UILocationProvider: LocationName Dropdown="MainStack"]`
    * **MainMenu Panel** `[UIPanel: PanelId Dropdown="MainMenu"]`
    * **Settings Panel** `[UIPanel: PanelId Dropdown="SettingsMenu"]`
  * **Overlays GameObject** `[UILocationProvider: LocationName Dropdown="Overlays"]`
    * **Loading Screen** `[UIPanel: PanelId Dropdown="LoadingScreen", RegisterWithRouter=false]`

> [!TIP]
> You can easily detach a `UIPanel` from the global router by unchecking the `RegisterWithRouter` toggle in the inspector. This is perfect for local reactive UI elements that only listen to state!

### 3. Bootstrap the Architecture

Register everything centrally when your application boots.

```csharp
using UnityEngine;
using Aim4code.NanoServiceFlow;
using Aim4code.NanoServiceFlow.UI;

public class AppBootstrapper : MonoBehaviour 
{
    private void Awake() 
    {
        // 1. Core routing
        ServiceLocator.RegisterService<UIRouterService>();

        // 2. Your specific UI
        ServiceLocator.RegisterState(new MainUIState());
        ServiceLocator.RegisterService<MainUIService>();

        // 3. Start the Engine
        ServiceLocator.InitializeAll();
    }
}
```

### 4. Dispatch Navigation Actions

Any system, game flow, or UI button can now open a screen by dispatching a simple intent. The `UIRouterService` will automatically figure out which canvas and stack it belongs to.

```csharp
using Aim4code.NanoServiceFlow;
using Aim4code.NanoServiceFlow.UI;

// Push to a stack
ServiceLocator.Dispatch(new OpenScreenAction("SettingsMenu"));

// Toggle an independent overlay
ServiceLocator.Dispatch(new ToggleOverlayAction("MainUI", "LoadingScreen", true));
```

## Architectural Philosophy

NanoServiceFlow.UI diverges from traditional MVC models by embracing a **Unidirectional Data Flow**. Views (Panels) cannot mutate the state directly; they only observe the `ActivePanels` hashset. State mutations happen entirely via pure struct `Actions` intercepted by `[Reducer]` methods in the Service layer. The `UIRouterService` acts as a Mediator, translating generic screen requests into exact system coordinates, guaranteeing race-condition-free, highly predictable user interfaces.

## License

MIT License. See `LICENSE.md` for more information.
