// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using System.Reflection;
using Aim4code.NanoServiceFlow;
using UnityEngine;

namespace Aim4code.NanoServiceFlow.UI.Tests
{
    // 1. Mock State
    public class TestUIState : UIRootState { }

    // 2. Mock Service
    public class TestUIService : UIServiceBase<TestUIState>
    {
        public TestUIService(TestUIState state) : base(state, "TestUI") { }
    }

    // 3. Mock Provider for Component testing
    public class TestUIProvider : UIRootProvider
    {
        public override UIRootState GetState() => ServiceLocator.Get<TestUIState>();
    }
}