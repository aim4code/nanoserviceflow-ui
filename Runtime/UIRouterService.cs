// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using System.Collections.Generic;
using UnityEngine;
using Aim4code.NanoServiceFlow;

namespace Aim4code.NanoServiceFlow.UI
{
    public class UIRouterService : IInitializable
    {
        private readonly Dictionary<string, (string Root, string Location)> _routingTable = new();

        public void Initialize() { }

        [Reducer]
        public void OnRegisterRoute(RegisterUIRouteAction action)
        {
            _routingTable[action.PanelId] = (action.RootKey, action.Location);
        }

        [Reducer]
        public void OnUnregisterRoute(UnregisterUIRouteAction action)
        {
            _routingTable.Remove(action.PanelId);
        }

        [Reducer]
        public void OnOpenScreen(OpenScreenAction action)
        {
            if (_routingTable.TryGetValue(action.PanelId, out var route))
            {
                ServiceLocator.Dispatch(new PushPanelAction(route.Root, route.Location, action.PanelId));
            }
            else
            {
                Debug.LogError($"[UI Router] Cannot open '{action.PanelId}'. It is not registered in any active scene.");
            }
        }
    }
}