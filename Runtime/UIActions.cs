// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using Aim4code.NanoServiceFlow;

namespace Aim4code.NanoServiceFlow.UI
{
    // --- System Actions (Processed by specific UI Services) ---
    public readonly struct PushPanelAction : IAction
    {
        public readonly string RootKey;
        public readonly string Location;
        public readonly string PanelId;
        public PushPanelAction(string rootKey, string location, string panelId) 
        { RootKey = rootKey; Location = location; PanelId = panelId; }
    }

    public readonly struct PopPanelAction : IAction
    {
        public readonly string RootKey;
        public readonly string Location;
        public PopPanelAction(string rootKey, string location) 
        { RootKey = rootKey; Location = location; }
    }

    public readonly struct ToggleOverlayAction : IAction
    {
        public readonly string RootKey;
        public readonly string PanelId;
        public readonly bool Show;
        public ToggleOverlayAction(string rootKey, string panelId, bool show) 
        { RootKey = rootKey; PanelId = panelId; Show = show; }
    }

    // --- Designer Actions (Processed by the UIRouterService) ---
    public readonly struct OpenScreenAction : IAction
    {
        public readonly string PanelId;
        public OpenScreenAction(string panelId) => PanelId = panelId;
    }

    // --- Lifecycle Actions (Triggered by Scene loading/unloading) ---
    public readonly struct RegisterUIRouteAction : IAction
    {
        public readonly string PanelId;
        public readonly string RootKey;
        public readonly string Location;
        public RegisterUIRouteAction(string panelId, string rootKey, string location)
        { PanelId = panelId; RootKey = rootKey; Location = location; }
    }

    public readonly struct UnregisterUIRouteAction : IAction
    {
        public readonly string PanelId;
        public UnregisterUIRouteAction(string panelId) => PanelId = panelId;
    }
}