# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-03-27

### Added

- Initial release of the `NanoServiceFlow.UI` core architecture.
- `UIRootState` to track navigation per layer/location via `LocationStacks` and reactive `ActivePanels`.
- `UIRouterService` for dynamic scene-based auto-discovery of UI panels.
- Action structs for system-level routing (`PushPanelAction`, `PopPanelAction`, `ToggleOverlayAction`).
- Action structs for intent-level routing (`OpenScreenAction`).
- `UIRootProvider` and `UILocationProvider` components to allow hierarchy-based dependency resolution.
- `UIPanel` component handling safe `CanvasGroup` raycast toggling and state-bound visibility.
- `UIPanel` now includes a `RegisterWithRouter` toggle for pure reactive UI elements that bypass global navigation.
- `IUITransition` interface to integrate with `UniTask` for asynchronous visual animations.
- `UIRoutingDatabase` ScriptableObject to systematically configure Roots, Locations, and Panels centrally.
- `[UIRootDropdown]`, `[UILocationDropdown]`, and `[UIPanelDropdown]` attributes alongside Editor Property Drawers to completely eliminate magic strings by replacing them with inspector dropdowns.

### Changed

- Refactored `UIRootProvider` so that `RootKey` is now a generic serialized field configured via inspector dropdowns rather than an abstract C# property override.