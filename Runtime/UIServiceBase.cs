// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using System.Collections.Generic;
using Aim4code.NanoServiceFlow;

namespace Aim4code.NanoServiceFlow.UI
{
    public abstract class UIServiceBase<TState> : IInitializable where TState : UIRootState
    {
        protected readonly TState _state;
        protected readonly string _rootKey;

        protected UIServiceBase(TState state, string rootKey)
        {
            _state = state;
            _rootKey = rootKey;
        }

        public virtual void Initialize() { }

        [Reducer]
        public void OnPushPanel(PushPanelAction action)
        {
            if (action.RootKey != _rootKey) return;
            
            var panels = new HashSet<string>(_state.ActivePanels.Value);
            
            if (!_state.LocationStacks.ContainsKey(action.Location))
                _state.LocationStacks[action.Location] = new List<string>();

            var stack = _state.LocationStacks[action.Location];

            // Hide the current top panel of this specific stack
            if (stack.Count > 0)
            {
                panels.Remove(stack[stack.Count - 1]);
            }

            stack.Add(action.PanelId);
            panels.Add(action.PanelId);
            
            _state.ActivePanels.Value = panels;
        }

        [Reducer]
        public void OnPopPanel(PopPanelAction action)
        {
            // 1. Ignore if it's not for this Canvas
            if (action.RootKey != _rootKey) return;

            // 2. Ignore if the location doesn't exist or is already empty
            if (!_state.LocationStacks.TryGetValue(action.Location, out var stack) || stack.Count == 0) return;

            // 3. Create a fresh copy of the active panels to safely modify
            var nextActivePanels = new System.Collections.Generic.HashSet<string>(_state.ActivePanels.Value);

            // 4. Identify the panel we are removing and take it out of the stack and our new set
            string panelToRemove = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            nextActivePanels.Remove(panelToRemove);

            // 5. IF there is another panel underneath it, reveal it!
            if (stack.Count > 0)
            {
                string panelToReveal = stack[stack.Count - 1];
                nextActivePanels.Add(panelToReveal);
            }

            // 6. Assign the new collection back to the state. 
            // This assignment is what actually triggers the Views to run their fade animations!
            _state.ActivePanels.Value = nextActivePanels;
        }

        [Reducer]
        public void OnToggleOverlay(ToggleOverlayAction action)
        {
            if (action.RootKey != _rootKey) return;

            var panels = new HashSet<string>(_state.ActivePanels.Value);
            
            if (action.Show) panels.Add(action.PanelId);
            else panels.Remove(action.PanelId);
            
            _state.ActivePanels.Value = panels;
        }
    }
}