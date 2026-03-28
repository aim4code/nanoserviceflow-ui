// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using UnityEngine;

namespace Aim4code.NanoServiceFlow.UI
{
    public abstract class UIRootProvider : MonoBehaviour
    {
        [SerializeField, UIRootDropdown]
        private string _rootKey;

        public string RootKey => _rootKey;

        public abstract UIRootState GetState();
    }
}