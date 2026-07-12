# Changelog

All notable changes to this project will be documented in this file.

The format is based on both [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) and [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.2.1] - 2026-07-13

### Fixed

- **fix(editor):** the `UIPanelDropdown` (and sibling routing dropdowns) no longer overwrite serialized panel ids. The drawer previously wrote `property.stringValue` on every repaint and silently coerced a not-found value to the first list entry, so rendering the Inspector while the `UIRoutingDatabase` was empty or still importing (Unity startup, domain reload, or duplicating a panel) could rewrite authored ids and persist the wrong value on scene save. The drawer now writes only on real user selection, preserves unknown/empty values as a temporary `(missing!)` entry, wraps drawing in `BeginProperty`/`EndProperty`, and retries loading the database until the asset is available instead of latching a null result.

## [0.2.0] - 2026-07-09

### Changed

- **refactor!:** promoted the former `Samples/CommonUI` code into the package runtime, replacing the sample-folder dependency. Generic transitions (`FadeUITransition`, `AnimatorUITransition`, `MaterialPropertyUITransition`) moved to `Runtime/Transitions/` in the main assembly under the `Aim4code.NanoServiceFlow.UI` namespace. **BREAKING:** these types are no longer in `Aim4code.NanoServiceFlow.UI.Samples.CommonUI`.
- **refactor!:** the application shell (`AppUIState`/`AppUIService`/`AppUIProvider`, `AppUIActions`, `LoadingScreenUIView`, `AutoAdvanceUIView`, `VideoUIView`, `VideoUITransition`) moved to a new, separately-referenceable assembly `Aim4code.NanoServiceFlow.UI.App` (`Runtime/App/`) under the `Aim4code.NanoServiceFlow.UI.App` namespace. This keeps `SceneManagement`/`Video` opinions out of consumers that only need the navigation core. **BREAKING:** namespace changed from `Aim4code.NanoServiceFlow.UI.Samples.CommonUI`.
- **refactor:** the `package.json` sample entry now points at `Samples~/QuickStart` (properly hidden from the AssetDatabase until imported), instead of the former always-compiled `Samples/CommonUI` folder.
- **refactor:** `package.json` dependency on `com.aim4code.nanoserviceflow` bumped to `0.3.0`.

### Added

- **docs:** new `Quick Start` sample under `Samples~/QuickStart` — a self-contained, importable navigation example (UI root + two panels navigated via `OpenScreenAction`/`PopPanelAction`, `NavigationButton` helper) with a recipe for layering in the App-shell scene loader. Replaces the old always-compiled `Common UI` sample.

## [0.1.1] - 2026-03-30

### Added

- **ci:** created a reusable `setup-unity` composite action to bootstrap Unity Test environments cleanly across repositories
- **ci:** automatically detect and merge `.github/scoped-registries.json` to inject OpenUPM dependencies into the test manifest, fully omitting the need for complex YAML workflow inputs
- **ci:** configure `ci.yml` to use a stable `TestProject` directory, safely enabling `actions/cache` to drastically reduce Unity Engine asset-import times

### Fixed

- **fix:** add `com.unity.ugui` and `com.unity.modules.ui` dependencies to `package.json` to ensure `CanvasGroup` and built-in UI elements successfully compile in headless CI environments

## [0.1.0] - 2026-03-27

### Added

- **feat:** initial release of the `NanoServiceFlow.UI` core architecture
- **feat:** `UIRootState` to track navigation per layer/location via `LocationStacks` and reactive `ActivePanels`
- **feat:** `UIRouterService` for dynamic scene-based auto-discovery of UI panels
- **feat:** action structs for system-level routing (`PushPanelAction`, `PopPanelAction`, `ToggleOverlayAction`)
- **feat:** action structs for intent-level routing (`OpenScreenAction`)
- **feat:** `UIRootProvider` and `UILocationProvider` components to allow hierarchy-based dependency resolution
- **feat:** `UIPanel` component handling safe `CanvasGroup` raycast toggling and state-bound visibility
- **feat:** `RegisterWithRouter` toggle on `UIPanel` for pure reactive UI elements that bypass global navigation
- **feat:** `IUITransition` interface to integrate with `UniTask` for asynchronous visual animations
- **feat:** `UIRoutingDatabase` ScriptableObject to systematically configure Roots, Locations, and Panels centrally
- **feat:** `[UIRootDropdown]`, `[UILocationDropdown]`, and `[UIPanelDropdown]` attributes alongside Editor Property Drawers to eliminate magic strings

### Changed

- **refactor:** `UIRootProvider` component now uses a generic serialized field for `RootKey` configured via inspector dropdowns, replacing the abstract C# property override