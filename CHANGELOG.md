# Changelog

All notable changes to this project will be documented in this file.

The format is based on both [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) and [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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