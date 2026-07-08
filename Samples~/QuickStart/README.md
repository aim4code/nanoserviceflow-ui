# Quick Start

A minimal, self-contained navigation example for **NanoServiceFlow.UI**. It shows
the entire per-scene pattern — a UI root (state + service + provider), a composition
root, and buttons that navigate by dispatching actions — with **no `SetActive`, no
`Show()/Hide()`, no screen singletons**.

Two scripts are included:

| Script | Role |
|---|---|
| `QuickStartBootstrapper.cs` | The `MenuUIState` / `MenuUIService` / `MenuUIProvider` trio plus the composition root that registers them and opens the first screen. |
| `NavigationButton.cs` | A uGUI `Button` → `OpenScreenAction` / `PopPanelAction` bridge. |

> This sample ships as **code only**. Unity scenes/prefabs don't survive being
> hand-authored outside the editor, so instead of a prebuilt scene you get the
> reusable scripts and the 5-minute recipe below to wire them up. The result is
> the canonical "two panels, navigate between them" demo.

## Build the demo scene (~5 min)

1. **New scene.** Add a `Canvas` (with `EventSystem` — Unity adds one automatically
   with the first UI element) and set it to *Screen Space - Overlay*.
2. **Root.** On the `Canvas`, add the **`MenuUIProvider`** component and set its
   **Root Key** to `MenuUI` (matches the key in `MenuUIService`).
3. **Location.** Add a child empty `RectTransform` named `Main` stretched to fill
   the canvas, and add a **`UILocationProvider`** to it with its **Location** set to
   `Main`. All panels for this layer go under here.
4. **Home panel.** Under `Main`, add a full-screen `Image` named `Home Panel`. Add:
   - a **`CanvasGroup`**,
   - a **`UIPanel`** with **Panel Id** = `Home Panel`,
   - a **`FadeUITransition`** (from `Runtime/Transitions/`) for a fade in/out.
   Put a `Button` inside it, add **`NavigationButton`** (mode *OpenScreen*,
   Panel Id = `Settings Panel`).
5. **Settings panel.** Duplicate `Home Panel`, rename to `Settings Panel`, set its
   `UIPanel` **Panel Id** = `Settings Panel`. Give its button a **`NavigationButton`**
   in mode *PopPanel* (Root Key `MenuUI`, Location `Main`) so it goes back.
6. **Bootstrapper.** Create an empty GameObject `Bootstrapper`, add
   **`QuickStartBootstrapper`**, leave **Starting Panel Id** = `Home Panel`.
7. **Play.** `Home Panel` fades in; its button pushes `Settings Panel`; the Settings
   button pops back to `Home Panel`.

### About the routing database (optional but recommended)

The `Panel Id` fields above are plain strings here to keep the sample dependency-free.
In a real project, create a `UIRoutingDatabase` asset and the
`[UIRootDropdown]` / `[UILocationDropdown]` / `[UIPanelDropdown]` attributes turn all
those string fields into dropdowns — no more magic strings.

## Adding the App-shell (persistent canvas + scene loading)

The `Aim4code.NanoServiceFlow.UI.App` assembly (`Runtime/App/`) provides an optional,
opinionated application shell:

- `AppUIProvider` on a `DontDestroyOnLoad` canvas (root key `AppUI`) that survives
  scene loads and hosts a loading overlay.
- `AppUIService` whose `LoadSceneAction` side effect shows the loading panel, loads a
  scene with progress into `AppUIState.LoadingProgress`, then hides it.
- `LoadingScreenUIView`, `AutoAdvanceUIView` (splash/disclaimer timing) and
  `VideoUIView` / `VideoUITransition`.

To use it, register the app services alongside the scene ones in your bootstrapper:

```csharp
using Aim4code.NanoServiceFlow.UI.App;

ServiceLocator.RegisterService<UIRouterService>();
ServiceLocator.RegisterState(new AppUIState());
ServiceLocator.RegisterService<AppUIService>();
Instantiate(Resources.Load<GameObject>("AppUI")); // your persistent AppUI canvas prefab
// ...then your scene-specific root (MenuUIState/MenuUIService) as above.
```

Then dispatch `new LoadSceneAction("YourSceneName")` from a button to see the loading
overlay drive a real scene load.
