// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using System.Collections.Generic;
using Aim4code.NanoServiceFlow;

namespace Aim4code.NanoServiceFlow.UI
{
    public abstract class UIRootState
    {
        // Tracks navigation history per layer/location (e.g., "MainStack", "Modals")
        public Dictionary<string, List<string>> LocationStacks { get; } = new();
        
        // The reactive source of truth for ALL visible panels in this Root Canvas
        public ReactiveProperty<HashSet<string>> ActivePanels { get; } = new(new HashSet<string>());
    }
}