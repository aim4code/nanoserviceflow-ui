# Contributing to NanoServiceFlow - UI

First off, thank you for considering contributing to NanoServiceFlow! We aim to build a lightweight, pragmatic, and high-performance ecosystem for Unity.

## Getting Started

1. Fork the repository and create your branch from `main`.
2. Ensure you have a compatible Unity version (2021.3 LTS or newer) installed.
3. If you've added code that should be tested, add unit tests using the Unity Test Framework.
4. Ensure your code strictly follows the C# 9.0 compliance rule (consistent with the core repository).

## Architectural Guidelines

When contributing to the UI package, please keep these core principles in mind:

* **Zero Direct References:** MonoBehaviours should never directly reference Services. Communication must happen via `IAction` dispatches through the `ServiceLocator`.
* **State is Truth:** Views (`UIPanel`) must only react to changes in the `UIRootState`. They should not attempt to drive visual logic locally without the state dictating it first.
* **GC-Friendly:** Avoid LINQ or heavy allocations inside `[Reducer]` functions or reactive subscriptions, as UI navigation should be smooth and stutter-free.

## Pull Request Process

1. Ensure your code compiles without warnings in Unity.
2. Run the existing test suite and ensure nothing is broken.
3. Update the `README.md` or `CHANGELOG.md` with details of changes to the interface, if applicable.
4. Open a Pull Request! A maintainer will review your code, provide feedback, and eventually merge it.

## Code of Conduct

By participating in this project, you agree to abide by our Code of Conduct. Please be respectful, constructive, and helpful to your fellow developers.