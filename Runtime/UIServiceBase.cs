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
            if (action.RootKey != _rootKey) return;
            if (!_state.LocationStacks.ContainsKey(action.Location)) return;

            var stack = _state.LocationStacks[action.Location];
            if (stack.Count <= 1) return; // Prevent popping the base layer

            var panels = new HashSet<string>(_state.ActivePanels.Value);
            
            // Remove current top
            var currentTop = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            panels.Remove(currentTop);

            // Show the new top
            var newTop = stack[stack.Count - 1];
            panels.Add(newTop);

            _state.ActivePanels.Value = panels;
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