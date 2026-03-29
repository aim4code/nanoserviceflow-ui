// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using System.Collections.Generic;
using UnityEngine;

namespace Aim4code.NanoServiceFlow.UI
{
    [CreateAssetMenu(fileName = "UIRoutingDatabase", menuName = "NanoServiceFlowUI/Routing Database")]
    public class UIRoutingDatabase : ScriptableObject
    {
        [Header("Registered Roots (Canvases)")]
        public List<string> Roots = new();

        [Header("Registered Locations")]
        public List<string> Locations = new();

        [Header("Registered Panels")]
        public List<string> Panels = new();
    }
}