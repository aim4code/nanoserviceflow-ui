// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using System.Threading;
using UnityEngine;

namespace Aim4code.NanoServiceFlow.UI
{
    public class UILocationProvider : MonoBehaviour
    {
        [Tooltip("The name of this UI layer (e.g., 'Modals', 'MainStack', 'Overlays')")]
        public string LocationName;
        
        public SemaphoreSlim TransitionLock { get; } = new SemaphoreSlim(1, 1);
    }
}